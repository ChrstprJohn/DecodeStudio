# Portfolio branches

| Branch | Visitor access | Admin access |
| --- | --- | --- |
| `feature/school-login` | Login required by default. Visitors can register, sign in, and manage their own profile. | Existing admin sign-in, portfolio editing, and account management. |
| `feature/public-portfolio` | Anyone can open the portfolio. No visitor sign-in or registration. | Open `/admin` directly to sign in. Admins can edit the portfolio and their own profile. |

Both versions use the existing `ConnectionStrings:Portfolio` setting and the same
SQL schema. Point both deployments at the same database to share content and the
existing admin account. Portfolio edits, password changes, and account status
changes in that database affect both versions. Use a separate database for
classroom experiments that should not affect the live portfolio. Uploaded files
are stored outside SQL; use shared persistent upload storage or sync the files
if both deployments need to display new uploads.

## Moving shared changes between branches

Keep shared improvements (portfolio sections, content editors, and fixes)
in separate commits from branch-specific login and account changes. For example,
after committing a shared admin editor fix on the school branch, carry just
that commit over:

```powershell
git switch feature/public-portfolio
git cherry-pick <shared-commit-sha>
```

The same process works in reverse. Avoid merging one variant wholesale into the
other: its authentication, navigation, and account routes are deliberately
different. If a shared change touches those files, preserve the receiving branch's
access rules and recheck anonymous access and admin sign-in. Apply shared schema
changes compatibly with both deployed versions.
