# Security Policy

## Supported versions

Security fixes are applied to the current default branch. No separate release-support schedule is currently published.

## Reporting a vulnerability

Please do not publish exploitable vulnerability details in a public issue. Use GitHub's private vulnerability reporting feature for this repository if it is enabled, or email [abdullahzulfiqar137@gmail.com](mailto:abdullahzulfiqar137@gmail.com) with the affected component, impact, and safe reproduction steps.

Never include real database credentials, JWT signing keys, SMTP credentials, API keys, customer records, or production data in a report.

## Deployment safety

This project is an active-development software project. Before production use, operators must review authentication, authorization, tenant isolation, backup/restore procedures, secret storage, logging, and database recovery against their own deployment requirements. Do not expose development seed accounts or sample credentials publicly.
