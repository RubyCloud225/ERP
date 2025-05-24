# Security Policy

## Supported Versions

We currently support the following .NET versions:

| .NET Version | Supported | Notes |
|--------------|-----------|-------|
| .NET 9.0     | ✅ Yes     | Actively maintained and tested |
| Older (<9.0) | ❌ No      | Unsupported |
| Previews     | ⚠ Partial | Not officially supported or secure for production |

---

## Reporting a Vulnerability

If you discover a security vulnerability, **please do not create a public issue**.

Instead, use **[GitHub’s built-in security advisory system](https://github.com/RubyCloud/ERP/security/advisories/new)** to privately report vulnerabilities.

GitHub allows you to:

- Confidentially disclose security issues.
- Collaborate on a private patch if necessary.
- Receive CVE identifiers (if applicable).

🔗 [Submit a vulnerability report](../../security/advisories/new)

You may also contact us directly at **[your-email@example.com]** if you are unable to use GitHub’s reporting tools.

---

## Security Best Practices

- Run this project using **.NET 9.0** with the latest patches.
- Avoid using preview or nightly SDKs in production.
- Monitor dependencies and GitHub Dependabot alerts for new vulnerabilities.

---

## Versioning & Fixes

We follow [Semantic Versioning](https://semver.org). Security patches will be released as:

- **Patch updates** (e.g., `v1.2.3 → v1.2.4`) for backward-compatible fixes.
- Critical vulnerabilities may warrant immediate releases outside regular cadence.

---

*Thank you for helping keep this project and its users secure.*
