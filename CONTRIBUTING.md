# Contributing to Phone Registry System

## Git Workflow

### Branch Strategy

We use **Git Flow** branching model:

```
master (production)
  ↑
development (integration)
  ↑
feature/* (feature development)
hotfix/* (production fixes)
release/* (release preparation)
```

### Branch Types

#### 1. Master Branch
- **Purpose**: Production-ready code
- **Rules**: Only merge from `development` or `hotfix/*`
- **Protected**: Direct commits not allowed

#### 2. Development Branch
- **Purpose**: Integration branch for features
- **Rules**: Merge from `feature/*` branches
- **Testing**: All features tested here before release

#### 3. Feature Branches
- **Naming**: `feature/short-description`
- **Examples**:
  - `feature/contact-info-management`
  - `feature/report-system`
  - `feature/soft-delete`

#### 4. Release Branches
- **Naming**: `release/v1.0.0`
- **Purpose**: Prepare releases, bug fixes only

#### 5. Hotfix Branches
- **Naming**: `hotfix/critical-bug-fix`
- **Purpose**: Critical production fixes

### Commit Message Format

We follow **Conventional Commits** specification:

```
<type>[optional scope]: <description>

[optional body]

[optional footer(s)]
```

#### Types:
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting, etc.)
- `refactor`: Code refactoring
- `test`: Adding or modifying tests
- `chore`: Build process or auxiliary tool changes

#### Examples:
```
feat(contact): add contact info removal endpoint
fix(report): resolve async report generation issue
docs: update README with Docker instructions
test(person): add unit tests for person service
```

### Development Workflow

#### 1. Starting New Feature
```bash
git checkout development
git pull origin development
git checkout -b feature/your-feature-name
```

#### 2. Development
```bash
# Make changes
git add .
git commit -m "feat: add new functionality"
```

#### 3. Prepare for Merge
```bash
git checkout development
git pull origin development
git checkout feature/your-feature-name
git rebase development
```

#### 4. Create Pull Request
- Target: `development` branch
- Include: Description, testing notes
- Review: Required before merge

#### 5. Release Process
```bash
git checkout development
git checkout -b release/v1.0.0
# Final testing and bug fixes
git checkout master
git merge release/v1.0.0
git tag -a v1.0.0 -m "Release v1.0.0"
```

### Version Tagging

We use **Semantic Versioning** (SemVer):

- **MAJOR**: Breaking changes (`v2.0.0`)
- **MINOR**: New features (`v1.1.0`)
- **PATCH**: Bug fixes (`v1.0.1`)

### Pull Request Guidelines

#### Title Format:
```
[TYPE] Short description
```

#### Description Template:
```markdown
## What
Brief description of changes

## Why
Reason for the changes

## How
Technical approach used

## Testing
- [ ] Unit tests added/updated
- [ ] Integration tests passed
- [ ] Manual testing completed

## Checklist
- [ ] Code follows style guidelines
- [ ] Self-review completed
- [ ] Documentation updated
- [ ] No merge conflicts
```

### Code Review Process

1. **Self Review**: Author reviews own code
2. **Peer Review**: At least one team member reviews
3. **Testing**: All tests must pass
4. **Documentation**: Update if needed
5. **Approval**: Required before merge

### Hotfix Process

For critical production issues:

```bash
git checkout master
git checkout -b hotfix/critical-issue
# Fix the issue
git checkout master
git merge hotfix/critical-issue
git tag -a v1.0.1 -m "Hotfix v1.0.1: Critical issue fix"
git checkout development
git merge hotfix/critical-issue
```

## Quality Standards

- **Code Coverage**: Minimum 60%
- **Build**: Must pass all builds
- **Tests**: All tests must pass
- **Documentation**: Keep updated
- **Performance**: No significant regressions
