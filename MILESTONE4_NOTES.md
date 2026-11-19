# CologneWorld – Milestone 4: Security

## Implemented in this project
- ASP.NET Core Identity backed by the existing Entity Framework Core SQLite database.
- Customer registration and login using Identity password hashing (passwords are not stored as plain text).
- Password policy: minimum 8 characters, uppercase, lowercase, number and non-alphanumeric symbol.
- Unique email addresses and temporary account lockout after repeated failed login attempts.
- New public registrations are assigned the Customer role.
- Admin and Customer roles are created during database initialization.
- Admin product management is protected by `[Authorize(Roles = "Admin")]`.
- Admin can view registered users and assign either Customer or Admin roles from Manage Users.
- Login-required actions use `[Authorize]`; unauthorised users are redirected to the login page and forbidden users to Access Denied.
- Authentication cookies are HttpOnly, SameSite=Lax and Secure; HTTPS redirection and HSTS are configured for production.
- Logout is a POST action protected by an anti-forgery token.

## Presentation walkthrough
1. Open the store as a visitor and show the Sign Up and Login links.
2. Register a new customer with a strong password, then sign in.
3. Show that the signed-in account is displayed and Logout is available.
4. Sign in using the seeded administrator account configured in `appsettings.json`.
5. Show Manage Products and Manage Users. Open Manage Users and demonstrate changing a user's role.
6. Explain that the Admin controller checks the Admin role on the server, so hiding or guessing a link does not grant access.
7. Explain that HTTPS redirection and HSTS are enabled for deployment. Use an HTTPS URL during the security demonstration.

## Important deployment note
Change the seeded administrator password in configuration or user secrets before deploying. Do not publish production credentials in a public repository. Ensure the hosting environment has a valid TLS certificate and serves the site over HTTPS.
