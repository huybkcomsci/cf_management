# ✅ DEPLOYMENT CHECKLIST

**Project**: Cafe Management System
**Version**: 1.0.0
**Date**: ******\_\_\_\_******
**Deployed by**: ******\_\_\_\_******
**Platform**: ☐ Render ☐ Railway ☐ Azure

---

## PHASE 1: PRE-DEPLOYMENT (3-5 days before)

### Code Preparation

- [ ] All features completed and tested
- [ ] No console.WriteLine() for debugging
- [ ] Exception handling implemented properly
- [ ] No commented-out code remaining
- [ ] Code reviewed by team member

### Testing

- [ ] Local build successful: `dotnet build -c Release`
- [ ] Application runs locally
- [ ] Database migrations tested
- [ ] All pages/features functional
- [ ] Performance acceptable
- [ ] No errors in logs

### Database

- [ ] Migrations created: `dotnet ef migrations add <Name>`
- [ ] Migration SQL reviewed
- [ ] DEVELOPMENT database backed up
- [ ] Seed data prepared (if needed)

### Configuration

- [ ] appsettings.Production.json reviewed
- [ ] All required env vars documented
- [ ] Connection string verified with Supabase
- [ ] Logging levels set appropriately
- [ ] CORS configured

### Security Review (see SECURITY_CHECKLIST.md)

- [ ] No secrets in source code
- [ ] .env files in .gitignore
- [ ] appsettings.json doesn't contain passwords
- [ ] Security headers configured
- [ ] HTTPS enabled

---

## PHASE 2: FINAL PREPARATIONS (1 day before)

### Git Repository

- [ ] All changes committed: `git status` is clean
- [ ] Code pushed to main branch: `git push origin main`
- [ ] Repository is public/accessible to deployment platform
- [ ] Deployment files included:
  - [ ] Dockerfile
  - [ ] .dockerignore
  - [ ] appsettings.Production.json
  - [ ] scripts/ folder with shell scripts

### Supabase Database

- [ ] Database connection tested
- [ ] IP whitelist includes platform IPs (if applicable)
- [ ] SSL Mode set to Require
- [ ] Connection string copied and verified
- [ ] PRODUCTION database backed up

### Secrets Management

- [ ] All secrets written down securely
- [ ] Database password strong (16+ chars)
- [ ] No secrets in browser history/clipboard
- [ ] Secrets stored in vault/password manager

### Platform Account

- [ ] Account created and verified
- [ ] Payment method added (if paid tier needed)
- [ ] Organization/team set up (if applicable)
- [ ] Region selected (Singapore recommended for Asia)

### Team Communication

- [ ] Deployment window announced
- [ ] Team members informed of maintenance window
- [ ] Rollback plan communicated
- [ ] Support contact info shared

---

## PHASE 3: DEPLOYMENT (Deployment day)

### Start Deployment

**Time started**: ******\_\_\_\_******

### Build & Publish

- [ ] Docker image builds successfully (if using Docker)
  ```bash
  docker build -t cafe-management:latest .
  ```
- [ ] Published app size reasonable
- [ ] Build warnings reviewed
- [ ] No build errors

### Platform Setup

- [ ] Service created on platform
- [ ] Instance size correct
- [ ] Region selected properly
- [ ] Auto-restart enabled

### Environment Variables Set

- [ ] ASPNETCORE_ENVIRONMENT=Production
- [ ] ASPNETCORE_URLS=http://+:8080
- [ ] CONNECTION_STRING set correctly
- [ ] All other required vars added (see ENV_VARIABLES.md)
- [ ] Variables verified (not visible in logs)

### Deployment Initiated

- [ ] Deployment started
- [ ] No immediate errors in logs
- [ ] Build process running
- [ ] Container starting

### Monitoring During Deployment

**Deployment time**: ******\_\_\_\_****** ~ ******\_\_\_\_******

- [ ] Logs being monitored
- [ ] Build completing successfully
- [ ] App starting
- [ ] Database migrations running
- [ ] No crashes detected
- [ ] Waiting for app to be ready

**Status**: ☐ In Progress ☐ Complete ☐ Failed

---

## PHASE 4: VERIFICATION (Post-deployment)

### Health Checks

```bash
# Health endpoint
curl https://your-app-url/health
```

- [ ] Health endpoint returns 200 OK
- [ ] Response contains "healthy": true
- [ ] App is accessible via browser
- [ ] HTTPS works (no mixed content warnings)

### Database Connection

- [ ] Migrations applied successfully (check logs)
- [ ] Seed data loaded (if applicable)
- [ ] Database accessible from app
- [ ] No connection timeout errors

### Login & Authentication

- [ ] Can access login page
- [ ] Admin login works:
  - Email: admin@cafemanagement.local
  - Password: Admin@123
- [ ] Can navigate to dashboard
- [ ] Session works (stays logged in)
- [ ] Logout works

### Functionality Testing

- [ ] Home page loads
- [ ] Main features work
- [ ] API endpoints respond (if applicable)
- [ ] File uploads work (if applicable)
- [ ] Reports generate (if applicable)
- [ ] Export functionality works (if applicable)

### Performance

- [ ] Pages load reasonably fast
- [ ] No CPU spikes
- [ ] Memory usage normal
- [ ] No obvious performance issues

### Resources

- [ ] Static files loading (CSS, JS, images)
- [ ] Static files have correct mime types
- [ ] No 404 errors for assets
- [ ] CSS/styling appears correct
- [ ] JavaScript functions work

### Error Handling

- [ ] 404 errors show proper error page
- [ ] 500 errors don't expose stack traces
- [ ] Form validation works
- [ ] Error messages are user-friendly

### Security Headers

```bash
curl -I https://your-app-url | grep -i "strict-transport\|x-frame\|content-security"
```

- [ ] Strict-Transport-Security header present
- [ ] X-Frame-Options set to DENY
- [ ] Content-Security-Policy present
- [ ] X-Content-Type-Options present

### Logging

- [ ] Application logs visible
- [ ] Error logs populated (check for startup)
- [ ] Log level appropriate
- [ ] No sensitive data in logs

**Verification Status**: ☐ PASSED ☐ ISSUES FOUND

**Issues Found (if any)**:

```
1. ________________________________
2. ________________________________
3. ________________________________
```

---

## PHASE 5: POST-DEPLOYMENT

### Immediate Actions (Within 1 hour)

- [ ] Change admin password:
  - [ ] Original: Admin@123
  - [ ] New password: ******\_\_\_\_******
- [ ] Create backup of database:
  ```bash
  bash ./scripts/backup.sh
  ```
- [ ] Verify backup created successfully
- [ ] Notify team of successful deployment

### First Day Monitoring

- [ ] Monitor error rate and logs
- [ ] Check for any crashes
- [ ] User reports gathered
- [ ] All systems operational

### Documentation

- [ ] Deployment details recorded
- [ ] New deployment URL documented
- [ ] Access credentials secured
- [ ] Troubleshooting steps documented (if issues)

### Backup Verification

- [ ] Automatic backups scheduled (if available)
- [ ] Backup retention policy set
- [ ] Test restore procedure (not on prod!):
  ```bash
  bash ./scripts/restore.sh <backup_file>
  ```

### Monitoring Setup

- [ ] Alerts configured (if available)
- [ ] Error notifications enabled
- [ ] Health check monitoring active
- [ ] Performance monitoring active

### Team Communication

- [ ] Deployment success announced
- [ ] Users notified
- [ ] Team meeting held (if needed)
- [ ] Documentation updated

**Deployment completed successfully**: ☐ YES ☐ NO

**Final Status**: ******\_\_\_\_******
**Time completed**: ******\_\_\_\_******

---

## PHASE 6: ISSUES & ROLLBACK (If needed)

### Issue Encountered

**Issue**: **************\_\_\_\_**************
**Severity**: ☐ Critical ☐ Major ☐ Minor

**Solution attempted**:

```
_________________________________
_________________________________
```

### Rollback Procedure (If needed)

- [ ] Issue confirmed as critical
- [ ] Rollback decision made
- [ ] Previous version identified
- [ ] Rollback executed:
  ```bash
  git revert <commit_hash>
  git push origin main
  # Or redeploy previous version
  ```
- [ ] App restarted
- [ ] Health verified
- [ ] Team notified
- [ ] Root cause analysis scheduled

**Rollback Status**: ☐ N/A ☐ Successful ☐ Failed

---

## PHASE 7: FOLLOW-UP (1-7 days post)

### Daily Review

- [ ] Day 1: Check logs for errors
- [ ] Day 2: Monitor user reports
- [ ] Day 3: Check performance metrics
- [ ] Day 7: Full review

### Performance Review

- [ ] CPU usage stable
- [ ] Memory usage acceptable
- [ ] Database queries performant
- [ ] No timeouts or disconnects
- [ ] User experience satisfactory

### Security Review

- [ ] No suspicious login attempts
- [ ] No unauthorized access
- [ ] No security warnings
- [ ] SSL certificate valid
- [ ] HTTPS working correctly

### Business Review

- [ ] All features working
- [ ] Expected functionality delivered
- [ ] User feedback positive
- [ ] No critical issues
- [ ] Deployment meets requirements

### Post-Deployment Tasks

- [ ] Create follow-up PRs (if issues found)
- [ ] Update documentation
- [ ] Team retrospective (if applicable)
- [ ] Plan next deployment

---

## ROLLBACK CHECKLIST (Plans to revert quickly)

**If deployment fails seriously:**

### Step 1: Stop Current Service

```bash
# Platform-specific commands
# Render: Pause service in dashboard
# Railway: railway restart
# Azure: az webapp stop
```

### Step 2: Restore Previous Version

```bash
git checkout <previous-working-commit>
git push origin main
# Or re-deploy previous Docker image
```

### Step 3: Verify Previous Version

- [ ] Health check passes
- [ ] App loads
- [ ] Database restored if needed

### Step 4: Analyze Issue

- [ ] Identify cause in logs
- [ ] Document problem
- [ ] Plan fix

### Step 5: Communicate

- [ ] Notify team
- [ ] Inform users
- [ ] Set new deployment time

---

## SIGN-OFF

**Deployment Manager**: ************\_************ **Date**: ******\_******

**Verified by**: ************\_************ **Date**: ******\_******

**Approved by**: ************\_************ **Date**: ******\_******

**Notes**:

```
_____________________________________________________________________________

_____________________________________________________________________________

_____________________________________________________________________________
```

---

## QUICK REFERENCE LINKS

- Documentation index: `DOCS_INDEX.md`
- Quick start: `QUICK_START.md`
- Platform guide: `DEPLOY_*.md`
- Environment vars: `ENV_VARIABLES.md`
- Security: `SECURITY_CHECKLIST.md`

---

**Deployment Date**: ******\_\_\_\_******
**Environment**: ☐ Development ☐ Staging ☐ Production
**Result**: ☐ SUCCESS ☐ PARTIAL ☐ FAILED ☐ ROLLED BACK

**This checklist should be completed and signed before going live!**
