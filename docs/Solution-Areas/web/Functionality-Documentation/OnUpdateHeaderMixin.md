# OnUpdateHeaderMixin

These docs will assume an understanding of the [EventBus](event-bus.md) as this mixin listens to it for `UPDATE_HEADER` events.

## The payload

The callback takes three parameters for its payload:

- **`newHeader`**

  It can be either an object, in which case it's expected to contain the properties `captionKey`, `captionSize` and `headerKey`, or a string, where the `newHeader` itself is assumed to be the `headerKey`.

  All properties are expected to be strings, whilst `headerKey` and `captionKey` can also be callback functions, accepting the Vuex store and Vue-i18n instances to be passed as arguments, with the result being assigned to the relevant component properties.

  The `captionSize` _should_ be one of the values defined in the **src / lib / caption-size.js** constants.

- **`localised`**

  A boolean indicating whether or not the `headerKey` and `captionKey` have been localised.

- **`overrideShowContentHeader`**

  Whether the receiver should definitely show this header. The naming of this somewhat couples it to the current use case in ContentHeader, but essentially indicates this is an important header, so it should be shown.

## Example use cases

```js
// locale
export default {
  headers: { one: 'Bacon', two: 'Cheese' },
  captions: { three: 'Lettuce', four: 'Tomato' },
};

// store
export default {
  state: {
    this: 'headers.one',
    that: 'captions.three'
  }
}

// Sender
EventBus.$emit(ON_UPDATE_HEADER, {
  headerKey: (store, i18n) => (i18n.t(store.state.this)),
  captionKey: (store, i18n) => (i18n.t(store.state.that)),
  captionSize: CaptionSize.ExtraLarge,
});

// Receiver
console.log(this.header);                     // 'Bacon'
console.log(this.caption);                    // 'Lettuce'
console.log(this.captionSize);                // 'nhsuk-caption-xl'
console.log(this.overrideShowContentHeader);  // false
```

```js
// Route
const DOCUMENT_VIEW_ATTACHMENT = {
  path: 'documents/view-attachment',
  meta: {
    shouldShowContentHeader: false,
  },
};

// Sender
EventBus.$emit(ON_UPDATE_HEADER, 'Letter from Dan Jones', true, true);

// Receiver
console.log(this.header);                     // 'Letter from Dan Jones'
console.log(this.caption);                    // ''
console.log(this.overrideShowContentHeader);  // true