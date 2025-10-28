# Task 018: Campaign List & Navigation UI

## Description
Build the campaign list page with filtering, sorting, pagination, and navigation to individual campaign details. Include campaign status indicators, recent activity, and quick actions.

## Dependencies
- Task 002: Frontend Scaffolding
- Task 014: Frontend API Client & SDK Generation

## Technical Requirements

### Page Structure
**`app/campaigns/page.tsx`:**
- Campaign list page (landing page)
- Header with "Create Campaign" button
- Filter and sort controls
- Campaign grid or list view
- Pagination controls
- Empty state for no campaigns

### Campaign List Component
**`components/campaign/CampaignList.tsx`:**
- Render campaign items in grid or list layout
- Support view toggle (grid/list)
- Responsive design
- Loading skeleton states
- Empty state component

**List Item Display:**
- Campaign name (clickable to details)
- Status badge (Draft, Generating, Generated, Completed, Failed, Cancelled)
- Brief summary (truncated)
- Created/updated timestamps
- Quick action buttons (View, Delete)
- Artifact completion indicators (3/3 artifacts)

### Campaign Card Component
**`components/campaign/CampaignCard.tsx`:**
- Card layout for grid view
- Campaign name as heading
- Status badge with color coding
- Brief excerpt (2-3 lines)
- Artifact progress (e.g., "Copy ✓ | Short Copy ✓ | Visual ⏳")
- Audit status indicator
- Timestamps (created, last updated)
- Action buttons (View, More options menu)

**Status Badge Colors:**
- Draft: Gray
- Generating: Blue (with spinner)
- Generated: Yellow
- Completed: Green
- Failed: Red
- Cancelled: Gray

### Filter and Sort Controls
**`components/campaign/CampaignFilters.tsx`:**

**Filters:**
- Status filter (multi-select: All, Draft, Generating, Completed, Failed)
- Date range filter (Created in last: 7 days, 30 days, 90 days, All time)
- Clear filters button

**Sort Options:**
- Sort by: Name (A-Z, Z-A), Created (Newest, Oldest), Updated (Newest, Oldest)
- Sort direction toggle

**Filter State Management:**
```typescript
const [filters, setFilters] = useState({
  status: [],
  dateRange: 'all',
  sortBy: 'updated',
  sortOrder: 'desc',
});

const { data, isLoading } = useCampaigns({
  page: currentPage,
  pageSize: 12,
  status: filters.status,
  sortBy: filters.sortBy,
  sortOrder: filters.sortOrder,
});
```

### Pagination
**`components/common/Pagination.tsx`:**
- Page number buttons (show 5 pages max with ellipsis)
- Previous/Next buttons
- Page size selector (12, 24, 48)
- Total count display
- Keyboard navigation support

**Pagination Logic:**
```typescript
const {
  data: campaignsData,
  isLoading,
  error,
} = useCampaigns({
  page,
  pageSize,
  ...filters,
});

const totalPages = Math.ceil((campaignsData?.totalCount ?? 0) / pageSize);
```

### Search Functionality
**Search Bar:**
- Text input for campaign name search
- Debounced search (300ms)
- Clear search button
- Search icon
- Placeholder text

**Search Implementation:**
```typescript
const [searchQuery, setSearchQuery] = useState('');
const debouncedSearch = useDebounce(searchQuery, 300);

const { data } = useCampaigns({
  page,
  pageSize,
  searchQuery: debouncedSearch,
  ...filters,
});
```

### View Toggle
**`components/campaign/ViewToggle.tsx`:**
- Toggle between grid and list view
- Persist preference in localStorage
- Icons for grid/list views

### Quick Actions
**Campaign Item Actions:**
- View (navigate to details)
- Delete (confirmation modal)
- Duplicate (future)
- Export (future)

**Delete Confirmation:**
- Modal dialog
- Campaign name confirmation
- Warning message
- Cancel and Delete buttons

### Empty States
**No Campaigns State:**
- Illustration or icon
- "No campaigns yet" heading
- "Create your first campaign" button
- Brief explanation

**No Results State:**
- "No campaigns match your filters"
- Suggestion to adjust filters
- Clear filters button

### Loading States
**Initial Load:**
- Skeleton cards/rows (6-12 placeholders)
- Shimmer effect

**Pagination Load:**
- Show loading spinner
- Disable pagination controls
- Fade transition between pages

### Error States
**Error Handling:**
- Display error message banner
- Retry button
- Fallback to cached data if available
- Network error specific messaging

### Navigation
**Routes:**
- `/campaigns` - Campaign list page
- `/campaigns/new` - Create campaign page
- `/campaigns/[id]` - Campaign details page

**Navigation Patterns:**
- Click campaign card → navigate to details
- "Create Campaign" button → navigate to creation form
- Browser back button support
- Breadcrumbs (optional)

### Performance Optimizations
**Optimizations:**
- Virtualization for large lists (if 100+ campaigns)
- Image lazy loading (if thumbnails added)
- Memoize filter computations
- Debounce search input
- Optimize re-renders with React.memo

### Responsive Design
**Breakpoints:**
- Mobile (<640px): Single column list
- Tablet (640px-1024px): 2-column grid
- Desktop (>1024px): 3-4 column grid
- Adjust spacing and font sizes

### Accessibility
**Accessibility Features:**
- Semantic HTML (nav, main, article elements)
- Keyboard navigation for all controls
- ARIA labels for icon buttons
- Screen reader announcements for status changes
- Focus management for modals
- Skip links for main content

## Acceptance Criteria
- [ ] Campaign list displays with grid and list view options
- [ ] Status badges with correct color coding
- [ ] Filter by status and date range working
- [ ] Sort by name, created, updated working
- [ ] Search with debouncing functional
- [ ] Pagination with page size control
- [ ] Delete campaign with confirmation
- [ ] Empty state for no campaigns
- [ ] Loading states for initial load and pagination
- [ ] Error handling with retry
- [ ] Responsive design on all screen sizes
- [ ] Navigation to campaign details working
- [ ] View preference persisted in localStorage
- [ ] Accessibility compliance (WCAG 2.2 AA)

## Testing Requirements
- [ ] Unit tests for list and card components (≥85% coverage)
- [ ] Unit tests for filter and sort logic
- [ ] Unit tests for pagination logic
- [ ] Test search debouncing
- [ ] Test delete confirmation flow
- [ ] Integration tests with API data
- [ ] E2E tests for campaign list navigation
- [ ] Test empty and error states
- [ ] Test responsive layouts
- [ ] Test keyboard navigation
- [ ] Test screen reader compatibility

## Non-Functional Requirements
- Initial page load <2s
- Search results appear <500ms after typing stops
- Filter/sort updates <200ms
- Smooth pagination transitions
- Support for 100+ campaigns with virtualization
- WCAG 2.2 AA compliance

## Out of Scope
- Campaign templates gallery (future)
- Bulk operations (bulk delete, bulk approve) (future)
- Campaign tagging/categories (future)
- Advanced search with multiple fields (future)
- Campaign analytics/metrics (future)
- Export list to CSV (future)

## Notes
- Follow AGENTS.md frontend patterns
- Use Shadcn/ui components for UI consistency
- Implement proper semantic HTML structure
- Cache campaign list data appropriately
- Consider infinite scroll as alternative to pagination (based on UX testing)
- Test with various campaign counts (0, 1, 10, 100+)
- Document filter and sort behavior
- Create reusable pagination component
- Ensure proper focus management
- Consider adding campaign thumbnails in future
