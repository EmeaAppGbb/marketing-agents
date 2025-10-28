# Task 016: Campaign Creation UI & Form

## Description
Build the campaign creation user interface with a comprehensive form for entering campaign briefs, tone guidelines, brand assets, and platform selections. Support form validation, draft saving, and submission to trigger orchestration.

## Dependencies
- Task 002: Frontend Scaffolding
- Task 014: Frontend API Client & SDK Generation

## Technical Requirements

### Form Components
Create campaign creation form using React Hook Form + Zod:

**Form Sections:**
1. Campaign Basic Info
   - Campaign name (required)
   - Campaign objective (textarea, required)
   
2. Target Audience
   - Audience description (textarea, required)
   
3. Product Details
   - Product name (required)
   - Product description (textarea, required)
   
4. Brand & Tone
   - Tone guidelines (multi-select chips: Professional, Casual, Playful, Authoritative, etc.)
   - Custom tone descriptors (optional text input)
   - Brand color palette (color picker inputs, optional, hex values)
   
5. Platform Selection (for Short Copy)
   - Platform checkboxes: Twitter/X, Facebook, Instagram, LinkedIn, Threads
   - At least one platform required
   
6. Constraints (Optional)
   - Prohibited terms (comma-separated input)
   - Length preferences (radio: All, Short only, Medium only, Long only)
   - Accessibility emphasis toggle
   
7. Execution Mode
   - Radio selection: Parallel (default) or Sequential
   - Explanation tooltips for each mode

### Zod Schema Validation
Create comprehensive validation schema:

**`lib/validation/campaignSchema.ts`:**
```typescript
import { z } from 'zod';

export const campaignBriefSchema = z.object({
  name: z.string().min(3, 'Name must be at least 3 characters').max(100),
  objective: z.string().min(20, 'Objective must be at least 20 characters').max(1000),
  targetAudience: z.string().min(10, 'Target audience must be at least 10 characters').max(500),
  productName: z.string().min(2, 'Product name required').max(100),
  productDetails: z.string().min(20, 'Product details must be at least 20 characters').max(1000),
  toneGuidelines: z.array(z.string()).min(1, 'Select at least one tone guideline'),
  customTone: z.string().max(200).optional(),
  brandPalette: z.array(z.string().regex(/^#[0-9A-Fa-f]{6}$/, 'Invalid hex color')).max(5).optional(),
  selectedPlatforms: z.array(z.enum(['Twitter', 'Facebook', 'Instagram', 'LinkedIn', 'Threads']))
    .min(1, 'Select at least one platform'),
  prohibitedTerms: z.string().optional(),
  lengthPreference: z.enum(['All', 'ShortOnly', 'MediumOnly', 'LongOnly']).default('All'),
  accessibilityEmphasis: z.boolean().default(false),
  executionMode: z.enum(['Parallel', 'Sequential']).default('Parallel'),
});

export type CampaignBriefFormData = z.infer<typeof campaignBriefSchema>;
```

### Form Component Structure
**`app/campaigns/new/page.tsx`:**
- Campaign creation page with form
- Multi-step wizard OR single scrollable form
- Progress indicator if multi-step
- Form state persistence (draft saving)

**`components/campaign/CampaignForm.tsx`:**
- Main form component
- Integration with React Hook Form
- Field-level validation feedback
- Submit and draft save actions

**Field Components:**
- `CampaignNameInput` - Text input with character counter
- `ObjectiveTextarea` - Expandable textarea with character counter
- `ToneSelector` - Multi-select chip component
- `ColorPaletteInput` - Color picker array input
- `PlatformSelector` - Platform checkbox grid
- `ExecutionModeRadio` - Radio group with explanations

### Form State Management
Use React Hook Form:
```typescript
const form = useForm<CampaignBriefFormData>({
  resolver: zodResolver(campaignBriefSchema),
  defaultValues: {
    toneGuidelines: [],
    selectedPlatforms: [],
    lengthPreference: 'All',
    accessibilityEmphasis: false,
    executionMode: 'Parallel',
  },
});
```

### Draft Saving
Implement auto-save and manual save:
- Auto-save to localStorage every 30 seconds
- Manual "Save Draft" button
- Load draft on page load if exists
- Clear draft on successful submission
- Show draft indicator in UI

**Draft Storage:**
```typescript
const DRAFT_KEY = 'campaign-draft';

const saveDraft = (data: Partial<CampaignBriefFormData>) => {
  localStorage.setItem(DRAFT_KEY, JSON.stringify({
    data,
    savedAt: new Date().toISOString(),
  }));
};

const loadDraft = (): Partial<CampaignBriefFormData> | null => {
  const draft = localStorage.getItem(DRAFT_KEY);
  if (!draft) return null;
  
  const { data, savedAt } = JSON.parse(draft);
  const age = Date.now() - new Date(savedAt).getTime();
  
  // Discard drafts older than 24 hours
  if (age > 24 * 60 * 60 * 1000) {
    localStorage.removeItem(DRAFT_KEY);
    return null;
  }
  
  return data;
};
```

### Form Submission
Handle form submission with API integration:

```typescript
const createCampaignMutation = useCreateCampaign();
const generateCampaignMutation = useGenerateCampaign();

const onSubmit = async (data: CampaignBriefFormData) => {
  try {
    // 1. Create campaign
    const campaign = await createCampaignMutation.mutateAsync({
      name: data.name,
      brief: {
        objective: data.objective,
        targetAudience: data.targetAudience,
        productDetails: `${data.productName}: ${data.productDetails}`,
        toneGuidelines: data.toneGuidelines,
        brandPalette: data.brandPalette,
        constraints: {
          prohibitedTerms: data.prohibitedTerms?.split(',').map(t => t.trim()),
          lengthPreferences: data.lengthPreference,
        },
      },
    });

    // 2. Trigger generation
    await generateCampaignMutation.mutateAsync({
      campaignId: campaign.id,
      executionMode: data.executionMode,
      selectedPlatforms: data.selectedPlatforms,
    });

    // 3. Clear draft
    localStorage.removeItem(DRAFT_KEY);

    // 4. Navigate to campaign page
    router.push(`/campaigns/${campaign.id}`);
  } catch (error) {
    // Show error toast
    toast.error('Failed to create campaign. Please try again.');
  }
};
```

### UI/UX Features
**Form Features:**
- Real-time validation with inline error messages
- Character counters for text inputs
- Color preview chips for palette
- Platform icons for visual selection
- Tooltips explaining each field
- Responsive design (mobile-friendly)
- Loading states during submission
- Disabled state during submission
- Success/error feedback

**Accessibility:**
- Proper label associations
- ARIA attributes for errors
- Keyboard navigation support
- Focus management
- Screen reader announcements

### Error Handling
Display validation and submission errors:
- Field-level validation errors below inputs
- Form-level error summary at top
- API error messages in toast notifications
- Network error handling with retry option

### Loading States
Show progress during operations:
- Disabled inputs during submission
- Loading spinner on submit button
- Progress indicator for multi-step form
- Draft saving indicator (subtle)

## Acceptance Criteria
- [ ] Campaign creation form with all required sections
- [ ] Zod validation schema enforcing all rules
- [ ] Real-time field validation with inline errors
- [ ] Character counters for text fields
- [ ] Color palette input with hex validation
- [ ] Platform selection with visual checkboxes
- [ ] Draft auto-save every 30 seconds
- [ ] Draft load on page mount if exists
- [ ] Form submission creates campaign and triggers generation
- [ ] Navigation to campaign page after successful submission
- [ ] Form resets and clears draft after submission
- [ ] Loading states during submission
- [ ] Error handling with user-friendly messages
- [ ] Responsive design working on mobile/tablet/desktop
- [ ] Accessibility compliance (WCAG 2.2 AA)

## Testing Requirements
- [ ] Unit tests for form components (≥85% coverage)
- [ ] Unit tests for validation schema
- [ ] Test draft save/load functionality
- [ ] Test form submission flow
- [ ] Test validation error display
- [ ] Test API error handling
- [ ] Integration tests with React Hook Form
- [ ] E2E tests for full campaign creation flow
- [ ] Test keyboard navigation
- [ ] Test screen reader compatibility
- [ ] Test responsive layouts

## Non-Functional Requirements
- Form renders in <1s
- Field validation feedback <100ms
- Draft save operation <50ms
- Form submission response <3s (including API calls)
- Mobile-responsive on screens ≥320px width
- WCAG 2.2 AA compliance

## Out of Scope
- Campaign templates (future)
- Multi-user collaboration on brief (future)
- AI-assisted brief generation (future)
- Image/media upload for brand assets (future)
- Campaign duplication from existing

## Notes
- Follow AGENTS.md frontend patterns
- Use Shadcn/ui components for consistent design
- Implement proper form field components
- Consider multi-step wizard vs single form based on UX testing
- Test draft persistence thoroughly
- Document form validation rules
- Create reusable form field components
- Ensure proper error message UX
- Use optimistic UI updates where appropriate
- Consider form analytics for completion rates
