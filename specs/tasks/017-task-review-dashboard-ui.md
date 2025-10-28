# Task 017: Review Dashboard UI - Artifact Display & Audit Results

## Description
Build the comprehensive review dashboard that displays all generated campaign artifacts (Copy, Short Copy, Visual Concepts) with their audit results, version history, and approval actions. Support artifact navigation, version comparison, and approval workflow.

## Dependencies
- Task 002: Frontend Scaffolding
- Task 014: Frontend API Client & SDK Generation
- Task 015: SignalR Real-Time Integration (Frontend)

## Technical Requirements

### Page Structure
**`app/campaigns/[id]/page.tsx`:**
- Main campaign review dashboard
- Real-time status updates via SignalR
- Artifact display sections
- Audit results panel
- Action buttons (approve, regenerate)

### Layout Components
Create three-panel layout:

**1. Campaign Header**
- Campaign name and brief summary
- Overall status indicator (Generating, Generated, Auditing, Completed)
- Progress indicator during generation
- Timestamp information

**2. Artifacts Panel (Main Content)**
- Tabbed or accordion interface for artifact types
- Copy section
- Short Social Copy section (sub-tabs per platform)
- Visual Concepts section
- Version history dropdown per artifact

**3. Audit Results Sidebar**
- Overall compliance status badge (Pass/Conditional/Fail)
- Category scores visualization
- Flagged items list
- Recommendations list
- Audit timestamp

### Artifact Display Components
Create artifact viewer components:

**`components/campaign/CopyArtifactView.tsx`:**
- Display headlines (all variants)
- Display body copy (short, medium, long)
- Display CTAs (all variants)
- Tone adherence metadata display
- Version selector dropdown
- Approve button
- Regenerate button with feedback form

**`components/campaign/ShortCopyArtifactView.tsx`:**
- Platform tabs (Twitter, Facebook, Instagram, LinkedIn, Threads)
- Per-platform variants display (≥3 per platform)
- Character count badges
- Over-limit warning indicators
- Hashtag suggestions display
- Alignment notes display
- Platform-specific regeneration

**`components/campaign/VisualConceptView.tsx`:**
- Concept cards (≥2 concepts)
- Thematic description
- Mood descriptors as tags
- Color palette visualization (swatches)
- Layout notes display
- Alt text display (accessibility)
- Concept differentiation indicator

### Audit Results Components
**`components/campaign/AuditReportPanel.tsx`:**
- Overall status badge with color coding
- Category scores as progress bars or radial charts
- Score thresholds visualization (Pass >80, Conditional 60-80, Fail <60)

**`components/campaign/FlaggedItemsList.tsx`:**
- Grouped by severity (Critical, High, Medium, Low)
- Severity badge with color coding
- Issue description
- Artifact reference link (click to scroll to artifact)
- Category tag

**`components/campaign/RecommendationsList.tsx`:**
- Priority badges (High, Medium, Low)
- Imperative phrasing recommendations
- Linked to flagged items
- Artifact reference for context

### Version History
**`components/campaign/VersionHistory.tsx`:**
- Dropdown or modal showing version list
- Version metadata: number, timestamp, status, audit status
- Compare versions feature (future)
- Revert to previous version (future)

### Real-Time Status Updates
Integrate SignalR for live updates:

**Status Progression:**
```typescript
const { latestEvent } = useCampaignEvents(campaignId);

// Update UI based on event state
useEffect(() => {
  if (!latestEvent) return;
  
  switch (latestEvent.state) {
    case 'GeneratingCopy':
      setStatus('Generating copywriting...');
      setCopyLoading(true);
      break;
    case 'GeneratingShortCopy':
      setStatus('Generating social media copy...');
      setShortCopyLoading(true);
      break;
    case 'GeneratingVisual':
      setStatus('Generating visual concepts...');
      setVisualLoading(true);
      break;
    case 'Auditing':
      setStatus('Auditing campaign artifacts...');
      setAuditLoading(true);
      break;
    case 'Completed':
      setStatus('Campaign generation complete!');
      setAllLoading(false);
      // Invalidate queries to fetch latest data
      refetchCampaign();
      break;
  }
}, [latestEvent]);
```

### Approval Workflow
**Approval Actions:**
- Individual artifact approval
- Bulk approve all button (if all audits pass)
- Approval persists across sessions
- Visual indication of approved artifacts
- Prevent regeneration of approved artifacts (warning)

**`components/campaign/ApprovalActions.tsx`:**
- Approve button (primary action)
- Regenerate button (secondary action)
- Approval status indicator
- Undo approval option (if not finalized)

### Regeneration Feedback Form
**`components/campaign/RegenerationForm.tsx`:**
- Triggered by "Regenerate" button
- Modal or drawer overlay
- Feedback text area (required, min 10 chars)
- Feedback tag multi-select (TooLong, TooShort, ChangeTone, etc.)
- Platform selector (for short copy only)
- Submit button
- Cancel button

**Regeneration Flow:**
1. User clicks "Regenerate" on artifact
2. Form opens with pre-filled context
3. User provides feedback
4. Submit triggers iteration mutation
5. Form closes, loading state shown
6. Real-time updates show regeneration progress
7. New version appears when complete

### Loading States
Show loading states during operations:
- Skeleton loaders for artifacts during generation
- Shimmer effect for pending sections
- Progress spinner for audit
- Disabled interactions during loading

### Empty States
Handle scenarios with no data:
- Campaign not yet generated (show brief summary, trigger generation button)
- Audit not yet complete (show pending state)
- No version history (show current version only)

### Error States
Display errors gracefully:
- Failed generation (show error message, retry button)
- Failed audit (show warning, manual audit trigger)
- Network errors (show offline indicator)

## Acceptance Criteria
- [ ] Dashboard displays all artifact types with proper formatting
- [ ] Audit results panel shows scores, flags, and recommendations
- [ ] Real-time status updates via SignalR working
- [ ] Artifact loading states during generation
- [ ] Version history accessible per artifact
- [ ] Approval workflow functional (approve, undo)
- [ ] Regeneration form with feedback capture
- [ ] Platform-specific short copy display with character counts
- [ ] Visual concept color palettes displayed
- [ ] Flagged items linkable to artifact sections
- [ ] Overall campaign status indicator accurate
- [ ] Responsive design on mobile/tablet/desktop
- [ ] Accessibility compliance (WCAG 2.2 AA)

## Testing Requirements
- [ ] Unit tests for all display components (≥85% coverage)
- [ ] Unit tests for approval actions
- [ ] Unit tests for regeneration form
- [ ] Test real-time event handling
- [ ] Test version history display
- [ ] Integration tests with API data
- [ ] E2E tests for approval workflow
- [ ] E2E tests for regeneration flow
- [ ] Test loading and error states
- [ ] Test responsive layouts
- [ ] Test accessibility with screen readers

## Non-Functional Requirements
- Dashboard initial load <2s (cached data)
- Artifact rendering <500ms
- Real-time event update latency <2s
- Smooth scrolling and interactions
- Support campaigns with 10+ versions per artifact
- WCAG 2.2 AA compliance

## Out of Scope
- Inline editing of artifacts (future)
- Visual diff comparison between versions (future)
- Bulk regeneration of multiple artifacts (future)
- Export to PDF/CSV (future)
- Collaboration comments (future)

## Notes
- Follow AGENTS.md review dashboard patterns
- Use Shadcn/ui components for consistent design
- Implement proper semantic HTML
- Use color-coding for audit statuses (green/yellow/red)
- Ensure flagged items are easily scannable
- Test with various campaign states (generating, completed, failed)
- Create reusable artifact display components
- Document component composition
- Consider virtualization for large version histories
- Ensure proper focus management for modals
