# Testing Security Headers - Quick Reference

## How to View Security Headers

### Method 1: Browser DevTools (Chrome/Edge)
1. Press `F12` to open DevTools
2. Go to **Network** tab
3. Navigate to `/Category` (or any page)
4. Click on the page request (usually the first one)
5. Click **Headers** tab
6. Scroll to **Response Headers**
7. Look for these headers:
   ```
   x-frame-options: DENY
   x-content-type-options: nosniff
   x-xss-protection: 1; mode=block
   content-security-policy: default-src 'self'; ...
   referrer-policy: strict-origin-when-cross-origin
   permissions-policy: geolocation=(), microphone=(), camera=()
   strict-transport-security: max-age=31536000; includeSubDomains; preload
   ```

### Method 2: PowerShell Command
```powershell
# Test locally
Invoke-WebRequest -Uri "https://localhost:7001/Category" -UseBasicParsing | Select-Object -ExpandProperty Headers

# Test production (replace with your URL)
Invoke-WebRequest -Uri "https://your-app.azurewebsites.net/Category" -UseBasicParsing | Select-Object -ExpandProperty Headers
```

### Method 3: cURL (if installed)
```bash
# Test locally
curl -I https://localhost:7001/Category

# Test production
curl -I https://your-app.azurewebsites.net/Category
```

## Online Security Checkers

### 1. SecurityHeaders.com ? Recommended
- URL: https://securityheaders.com
- Enter your production URL
- **Target Grade: A or A+**
- Provides detailed recommendations

### 2. Mozilla Observatory
- URL: https://observatory.mozilla.org
- Comprehensive security analysis
- **Target Score: 90+**
- Includes TLS and certificate checks

### 3. Google Safe Browsing Status
- URL: https://transparencyreport.google.com/safe-browsing/search
- Enter your production URL
- Should show: "No unsafe content found"

## What Each Header Does

| Header | Purpose | Setting |
|--------|---------|---------|
| `X-Frame-Options` | Prevents clickjacking | `DENY` |
| `X-Content-Type-Options` | Prevents MIME sniffing | `nosniff` |
| `X-XSS-Protection` | Browser XSS filter | `1; mode=block` |
| `Content-Security-Policy` | Controls resource loading | Custom policy |
| `Referrer-Policy` | Controls referrer info | `strict-origin-when-cross-origin` |
| `Permissions-Policy` | Browser feature permissions | Disabled sensitive features |
| `Strict-Transport-Security` | Forces HTTPS | 1 year, preload ready |

## Expected Results After Fix

### Before (Likely Missing)
```
Status: 200 OK
cache-control: no-cache, no-store
content-type: text/html; charset=utf-8
```

### After (With Security Headers)
```
Status: 200 OK
cache-control: no-cache, no-store
content-type: text/html; charset=utf-8
x-frame-options: DENY
x-content-type-options: nosniff
x-xss-protection: 1; mode=block
content-security-policy: default-src 'self'; script-src 'self' 'unsafe-inline' ...
referrer-policy: strict-origin-when-cross-origin
permissions-policy: geolocation=(), microphone=(), camera=()
strict-transport-security: max-age=31536000; includeSubDomains; preload
```

## Troubleshooting

### Headers Not Showing Up
1. **Clear browser cache**: Ctrl+Shift+Delete
2. **Hard refresh**: Ctrl+F5
3. **Check environment**: Headers are added for both Development and Production
4. **Verify deployment**: Make sure latest code is deployed

### CSP Blocking Resources
If Content-Security-Policy blocks legitimate resources:
1. Open Browser Console (F12 ? Console)
2. Look for CSP violation errors
3. Update the CSP policy in `Program.cs` to allow the resource
4. Example: Add `https://your-cdn.com` to the appropriate directive

### Still Getting Google Warning
1. **Wait 24-48 hours** - Google's cache needs to update
2. **Request review** - Go to Google Search Console
3. **Check for malware** - Run antivirus scan on hosting server
4. **Check SSL certificate** - Ensure valid and not expired

## Quick Test Checklist

After deploying:
- [ ] Visit your site in incognito mode
- [ ] Open DevTools ? Network ? Check Response Headers
- [ ] Confirm all 7 security headers are present
- [ ] Run SecurityHeaders.com scan
- [ ] Check Google Safe Browsing status
- [ ] No browser warnings on Category page
- [ ] All functionality still works (create/edit/delete categories)

## Google Safe Browsing Statuses

| Status | Meaning | Action |
|--------|---------|--------|
| ? No unsafe content found | Site is clean | None needed |
| ?? Site contains social engineering | False positive likely | Request review |
| ? Site contains harmful programs | Needs investigation | Scan for malware |
| ? Not enough data | Site too new | Wait, or request scan |

## Request Google Review (if needed)

1. Go to: https://search.google.com/search-console
2. Add/verify your property
3. Navigate to Security Issues
4. Click "Request Review"
5. Provide details about the fix
6. Submit and wait 24-72 hours

## Support Resources

- **Mozilla Security Guidelines**: https://infosec.mozilla.org/guidelines/web_security
- **OWASP Security Headers**: https://owasp.org/www-project-secure-headers/
- **Content-Security-Policy**: https://content-security-policy.com/
- **HSTS Preload**: https://hstspreload.org/
