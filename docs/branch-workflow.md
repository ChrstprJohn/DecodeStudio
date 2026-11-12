# Portfolio branches

| Branch | Visitor access | Admin access |
| --- | --- | --- |
| `feature/school-login` | Login required by default. Visitors can register, sign in, and manage their own profile. | Existing admin sign-in, portfolio editing, and account management. |
| `feature/public-portfolio` | Anyone can open the portfolio. No visitor sign-in or registration. | Open `/admin` directly to sign in. Admins can edit the portfolio and their own profile. |

Both versions use `ConnectionStrings:Portfolio` with dedicated SQL databases.
The public branch uses `portfolio` and reads its deployed connection from
`ConnectionStrings__Portfolio`. The school branch uses `portfolio_school` and
stores its local connection under the `decode-studio-portfolio-school` .NET user
secrets ID. Each branch checks the database name at startup.

To initialize the school database from an existing local `portfolio` database,
run `scripts/database/clone-school-portfolio.sql` from the school branch as a SQL
Server administrator. It restores a backup into `portfolio_school` and stops if
the destination already exists. The copy includes accounts and portfolio content
as of the backup. Uploaded files live outside SQL and need separate storage for
each running environment.

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
