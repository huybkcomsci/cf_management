# 🔒 Security Hardening Checklist for Production

## Pre-Deployment Security Review

### Database Security

- [ ] Database password is strong (16+ characters, mixed case, numbers, symbols)
- [ ] Database password is NOT stored in source code
- [ ] SSL/TLS encryption enabled for database connection (SSL Mode=Require)
- [ ] Database user permissions restricted (least privilege principle)
- [ ] Database backups encrypted and stored securely
- [ ] Connection pooling enabled and configured
- [ ] IP whitelist configured (if applicable)
- [ ] Database audit logging enabled
- [ ] Regular backups scheduled and tested

### Application Security

#### Authentication & Authorization

- [ ] Password requirements enforced (min 8 chars, complexity)
- [ ] Multi-factor authentication (optional but recommended)
- [ ] Default admin password changed immediately after deployment
- [ ] Session timeout configured (480 minutes / 8 hours)
- [ ] HTTPS-only cookies enforced
- [ ] CSRF protection enabled
- [ ] Role-based access control (RBAC) implemented
- [ ] API keys rotated regularly
- [ ] Service accounts have minimal required permissions

#### Secrets Management

- [ ] No secrets committed to Git
- [ ] `.env` files added to `.gitignore`
- [ ] Secrets stored in vault (Azure Key Vault, etc.)
- [ ] Environment variables used for all sensitive data
- [ ] Secrets not logged or exposed in error messages
- [ ] API keys masked in logs
- [ ] Rotation policy established (90-day rotation)

### Web Application Firewall

- [ ] HTTPS enforced (HTTP → HTTPS redirect)
- [ ] HSTS header enabled (max-age=31536000)
- [ ] Content Security Policy (CSP) configured
- [ ] X-Frame-Options set to DENY (prevent clickjacking)
- [ ] X-Content-Type-Options set to nosniff
- [ ] X-XSS-Protection header enabled
- [ ] Referrer-Policy configured
- [ ] Permissions-Policy configured
- [ ] Rate limiting implemented (optional)

### CORS (Cross-Origin Resource Sharing)

- [ ] CORS policy restricted to specific origins
- [ ] Wildcard origins (\*) NOT used in production
- [ ] Allowed methods specified explicitly
- [ ] Credentials handling properly configured
- [ ] Preflight requests timeout reasonable

### Input Validation & Output Encoding

- [ ] All user inputs validated server-side
- [ ] Input length limits enforced
- [ ] File uploads restricted (type, size)
- [ ] File uploads store outside webroot
- [ ] SQL injection prevention (parameterized queries)
- [ ] Cross-site scripting (XSS) prevention
- [ ] Output encoding performed (especially for user-generated content)
- [ ] Serialization properly configured

### Dependency Security

- [ ] All NuGet packages up-to-date
- [ ] Vulnerable packages identified and patched
- [ ] Dependency audit run regularly (`dotnet list package --outdated`)
- [ ] No pre-release packages in production
- [ ] No deprecated packages used
- [ ] License compliance verified
- [ ] Third-party libraries vetted

### Logging & Monitoring

- [ ] Logging configured (not too verbose, not too silent)
- [ ] Sensitive data NOT logged (passwords, API keys, PII)
- [ ] Logs stored securely
- [ ] Log retention policy configured (30-90 days)
- [ ] Structured logging enabled for analysis
- [ ] Error tracking configured (e.g., Application Insights)
- [ ] Failed login attempts monitored
- [ ] Access logs maintained

### Infrastructure Security

- [ ] HTTPS/TLS certificate installed and valid
- [ ] Certificate auto-renewal configured
- [ ] Server headers hardened
- [ ] Debug mode disabled in production
- [ ] Detailed error pages disabled
- [ ] Default pages removed/secured
- [ ] Unnecessary HTTP methods disabled (DELETE, TRACE, etc.)
- [ ] Database access restricted to application IP only
- [ ] Firewall rules configured

### Data Protection

- [ ] Sensitive data encrypted at rest (if applicable)
- [ ] Sensitive data encrypted in transit (TLS)
- [ ] Data retention policy implemented
- [ ] PII handling compliant with regulations (GDPR, etc.)
- [ ] Data backup retention policy defined
- [ ] Disaster recovery plan documented
- [ ] Data deletion request process established

### Deployment & CI/CD

- [ ] CI/CD pipeline configured securely
- [ ] GitHub Actions/DevOps secrets encrypted
- [ ] Deployment credentials rotated
- [ ] Code review process enforced (before merge)
- [ ] Automated security scanning enabled
- [ ] Signed commits enforced (optional)
- [ ] Repository access restricted
- [ ] Secrets not exposed in build logs

### API Security (if applicable)

- [ ] API authentication required (tokens, API keys)
- [ ] API rate limiting implemented
- [ ] API versioning strategy defined
- [ ] Deprecated APIs removed
- [ ] API documentation doesn't expose sensitive details
- [ ] Error messages don't reveal system details
- [ ] Request/response size limits enforced

### File Upload Security

- [ ] File types validated (whitelist approach)
- [ ] File size limited appropriately
- [ ] Files stored outside webroot
- [ ] File permissions restricted (non-executable)
- [ ] Virus/malware scanning (if applicable)
- [ ] Original filenames not preserved
- [ ] File serve mechanism doesn't allow directory traversal

## Post-Deployment Verification

### Automated Scans

- [ ] Run OWASP ZAP scan or similar
- [ ] Run dependency vulnerability scan
- [ ] Check SSL/TLS configuration (SSL Labs test)
- [ ] Review security headers (securityheaders.com)

### Manual Testing

- [ ] Test login with weak password (should fail)
- [ ] Test SQL injection attempts (should fail)
- [ ] Test XSS attempts (should fail)
- [ ] Test CSRF attempts (should fail)
- [ ] Verify HTTPS redirect works
- [ ] Verify security headers present
- [ ] Check SSL certificate validity
- [ ] Test CORS restrictions
- [ ] Verify rate limiting works (if implemented)

### Monitoring Setup

- [ ] Alert configured for failed logins
- [ ] Alert configured for database connection failures
- [ ] Alert configured for high error rates
- [ ] Alert configured for unusual traffic patterns
- [ ] Monitoring dashboard active
- [ ] Log aggregation working

## Security Headers Configuration

```csharp
// Verify these are set in Program.cs middleware:

// ✅ X-Content-Type-Options: nosniff
context.Response.Headers["X-Content-Type-Options"] = "nosniff";

// ✅ X-Frame-Options: DENY
context.Response.Headers["X-Frame-Options"] = "DENY";

// ✅ X-XSS-Protection: 1; mode=block
context.Response.Headers["X-XSS-Protection"] = "1; mode=block";

// ✅ Strict-Transport-Security: max-age=31536000
context.Response.Headers["Strict-Transport-Security"] =
    "max-age=31536000; includeSubDomains";

// ✅ Content-Security-Policy
context.Response.Headers["Content-Security-Policy"] =
    "default-src 'self'; script-src 'self' 'unsafe-inline' cdnjs.cloudflare.com; style-src 'self' 'unsafe-inline'";

// ✅ Referrer-Policy
context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

// ✅ Permissions-Policy
context.Response.Headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";
```

## Environment Variables Security Checklist

```bash
# ✅ Verify no secrets in:
grep -r "password" . --include="*.json" --include="*.cs"  # Should return 0 results
grep -r "password" . --include="*.csproj" --include="*.md"  # Documentation only

# ✅ Verify .env files ignored:
cat .gitignore | grep "\.env"  # Should exist

# ✅ Verify secrets not in logs:
grep -r "CONNECTION_STRING" . --include="*.cs" | grep -c "Console.WriteLine"  # Should be 0

# ✅ Verify environment-specific configs:
ls -la appsettings.*.json
# Should see: appsettings.json, appsettings.Development.json, appsettings.Production.json
```

## Incident Response Plan

In case of security breach:

1. **Immediate Actions**
   - [ ] Take application offline (if critical)
   - [ ] Notify security team/manager
   - [ ] Preserve logs and evidence
   - [ ] Document timeline

2. **Investigation**
   - [ ] Identify scope of breach
   - [ ] Review logs for suspicious activity
   - [ ] Check database for unauthorized access
   - [ ] Audit recent changes/deployments

3. **Containment**
   - [ ] Rotate all compromised passwords
   - [ ] Revoke affected tokens/keys
   - [ ] Update security rules
   - [ ] Patch vulnerable code

4. **Recovery**
   - [ ] Restore from backup if needed
   - [ ] Re-deploy patched version
   - [ ] Clear suspicious data
   - [ ] Resume normal operation

5. **Communication & Follow-up**
   - [ ] Notify affected users (if PII involved)
   - [ ] Post-incident review meeting
   - [ ] Update security policies
   - [ ] Plan security improvements

## Regular Maintenance Tasks

- [ ] **Weekly**: Review error logs for anomalies
- [ ] **Weekly**: Monitor security alerts
- [ ] **Monthly**: Update dependencies check
- [ ] **Monthly**: Rotate non-critical secrets
- [ ] **Quarterly**: Full security audit
- [ ] **Quarterly**: Penetration test (recommended for critical apps)
- [ ] **Quarterly**: Update security policies
- [ ] **Yearly**: Rotate critical passwords
- [ ] **Yearly**: Update SSL certificates
- [ ] **Yearly**: Full compliance audit

## Resources & References

- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [OWASP ASP.NET Core Security](https://owasp.org/www-project-secure-coding-practices-quick-reference-guide/)
- [Microsoft Security Best Practices](https://docs.microsoft.com/en-us/aspnet/core/security/)
- [NIST Cybersecurity Framework](https://www.nist.gov/cyberframework)
- [CWE Top 25](https://cwe.mitre.org/top25/)

## Approval & Sign-off

Before going live, ensure:

- [ ] Security checklist completed 100%
- [ ] All items tested and verified
- [ ] Security team review completed
- [ ] Final approval obtained

**Approved by**: ************\_************ **Date**: ******\_******

**Deployment authorized by**: ************\_************ **Date**: ******\_******
