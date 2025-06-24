# CSS Layers Migration Guide

A practical guide for migrating existing CSS to use CSS `@layer` rules, based on real-world experience with component libraries.

## Key Takeaways

### Component Library Configuration
Most component libraries require explicit configuration to support `@layer` rules:

```typescript
// Example: PrimeVue 4.x
app.use(PrimeVue, {
  theme: {
    preset: Aura,
    options: {
      cssLayer: {
        name: 'primevue',
        order: 'framework, primevue, pages'
      }
    }
  }
})
```

### Unlayered CSS Has Higher Priority
Any CSS outside `@layer` rules will override layered styles, regardless of specificity:

- **Wrap in lower-priority layer**: Move unlayered styles to a framework/vendor layer
- **Keep non-conflicting styles unlayered**: Icon fonts and utilities that don't conflict can stay outside layers for simplicity

### Sass @use Restrictions
`@use` cannot be nested inside `@layer` rules:

```scss
// ❌ This won't work
@layer framework {
  @use 'some-library/styles.css';
}

// ✅ Use @import instead
@layer framework {
  @import 'some-library/styles.css';
}

// ✅ Or keep @use at top level
@use 'some-library/styles.css';
```

### Define Layer Order Explicitly
Always declare your layer hierarchy upfront:

```scss
@layer framework, components, pages, utilities;
```

### Not Everything Needs Layers
Consider keeping these outside layers:
- Icon fonts (only affect pseudo-elements)
- Reset/normalize CSS
- Non-conflicting utility classes

## Quick Migration Checklist

1. Check if your component library supports CSS layers
2. Define explicit layer order
3. Move conflicting vendor CSS to lower-priority layers
4. Replace `@use` with `@import` inside layers
5. Keep non-conflicting styles unlayered if simpler