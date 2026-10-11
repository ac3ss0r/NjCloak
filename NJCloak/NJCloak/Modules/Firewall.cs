using NetFwTypeLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace NJCloak.Modules {

    public class FirewallRule {

        public string RuleName { get; private set; }
        public string Program { get; private set; }
        public bool Allow { get; private set; }
        public bool Enabled { get; private set; }

        public FirewallRule(string ruleName, string program, bool allow, bool enabled) {
            RuleName = ruleName;
            Program = program;
            Allow = allow;
            Enabled = enabled;
        }

        public string Title {
            get {
                return string.IsNullOrEmpty(Program) ? RuleName : Path.GetFileName(Program);
            }
        }
    }

    // Windows Firewall is driven through its own COM API (HNetCfg.FwPolicy2, netfw.h)
    // instead of spawning netsh.exe.
    public static class Firewall {

        private const string RulePrefix = "NJCloak-";

        private static readonly NET_FW_PROFILE_TYPE2_[] Profiles = {
            NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_DOMAIN,
            NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_PRIVATE,
            NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_PUBLIC
        };

        private static INetFwPolicy2 Policy {
            get {
                Type type = Type.GetTypeFromProgID("HNetCfg.FwPolicy2");
                if (type == null) {
                    throw new PlatformNotSupportedException(
                        "The Windows Firewall API (HNetCfg.FwPolicy2) is not available on this system.");
                }
                return (INetFwPolicy2)Activator.CreateInstance(type);
            }
        }

        public static bool IsAvailable {
            get { return Type.GetTypeFromProgID("HNetCfg.FwPolicy2") != null; }
        }

        public static List<FirewallRule> GetRules() {
            List<FirewallRule> rules = new List<FirewallRule>();
            foreach (object item in Policy.Rules) {
                INetFwRule rule = item as INetFwRule;
                if (rule == null) continue;
                string name = rule.Name;
                if (name == null || !name.StartsWith(RulePrefix, StringComparison.Ordinal)) continue;
                rules.Add(new FirewallRule(name, rule.ApplicationName,
                    rule.Action == NET_FW_ACTION_.NET_FW_ACTION_ALLOW, rule.Enabled));
            }
            rules.Sort(delegate (FirewallRule a, FirewallRule b) {
                return string.Compare(a.Title, b.Title, StringComparison.OrdinalIgnoreCase);
            });
            return rules;
        }

        // One rule per executable: adding an app that is already managed switches its action.
        public static void SetAppRule(string path, bool allow) {
            string ruleName = BuildRuleName(path);
            INetFwRules rules = Policy.Rules;
            try { rules.Remove(ruleName); }
            catch (COMException) { }

            Type type = Type.GetTypeFromProgID("HNetCfg.FWRule");
            if (type == null) {
                throw new PlatformNotSupportedException(
                    "The Windows Firewall API (HNetCfg.FWRule) is not available on this system.");
            }
            INetFwRule rule = (INetFwRule)Activator.CreateInstance(type);
            rule.Name = ruleName;
            rule.Description = "NJCloak: " + (allow ? "allow" : "block") + " outbound traffic for " + path;
            rule.ApplicationName = path;
            rule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT;
            rule.Action = allow ? NET_FW_ACTION_.NET_FW_ACTION_ALLOW : NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
            rule.Enabled = true;
            rule.Profiles = (int)NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_ALL;
            rules.Add(rule);
        }

        public static void RemoveRule(string ruleName) {
            Policy.Rules.Remove(ruleName);
        }

        public static bool IsWhitelistEnabled() {
            INetFwPolicy2 policy = Policy;
            foreach (NET_FW_PROFILE_TYPE2_ profile in Profiles) {
                if (policy.get_DefaultOutboundAction(profile) == NET_FW_ACTION_.NET_FW_ACTION_BLOCK) return true;
            }
            return false;
        }

        // Whitelist mode is the per-profile default action, not a catch all block rule:
        // block rules would take precedence over the allow rules of whitelisted apps.
        public static void SetWhitelistEnabled(bool enabled) {
            INetFwPolicy2 policy = Policy;
            NET_FW_ACTION_ outbound = enabled ? NET_FW_ACTION_.NET_FW_ACTION_BLOCK
                                              : NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
            foreach (NET_FW_PROFILE_TYPE2_ profile in Profiles) {
                policy.set_DefaultInboundAction(profile, NET_FW_ACTION_.NET_FW_ACTION_BLOCK);
                policy.set_DefaultOutboundAction(profile, outbound);
            }
        }

        public static void Reset() {
            INetFwPolicy2 policy = Policy;
            foreach (FirewallRule rule in GetRules()) {
                try { policy.Rules.Remove(rule.RuleName); }
                catch (COMException) { }
            }
            policy.RestoreLocalFirewallDefaults();
        }

        public static string GetGroupPolicyWarning() {
            if (Policy.LocalPolicyModifyState == NET_FW_MODIFY_STATE_.NET_FW_MODIFY_STATE_GP_OVERRIDE) {
                return "Group Policy overrides the firewall, so changing the default actions may not apply.";
            }
            return null;
        }

        private static string BuildRuleName(string path) {
            using (SHA1 sha = SHA1.Create()) {
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(path.Trim().ToLowerInvariant()));
                StringBuilder name = new StringBuilder(RulePrefix, RulePrefix.Length + 8);
                for (int i = 0; i < 4; i++) name.Append(hash[i].ToString("x2"));
                return name.ToString();
            }
        }
    }
}
