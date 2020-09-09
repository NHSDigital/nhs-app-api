# OnUpdateTitleMixin

This mixin works very much like the [`OnUpdateHeaderMixin`](on-update-header-mixin.md).

The differences being:

- listens for `UPDATE_TITLE` events
- the first payload parameter `newHeader` is `newTitle` with a single expected key - `titleKey` - and is used to set `this.title` on the component
- `overrideShowContentHeader` is not an expected payload parameter

See the [OnUpdateHeaderMixin examples](on-update-header-mixin.md#example-use-cases) for more information