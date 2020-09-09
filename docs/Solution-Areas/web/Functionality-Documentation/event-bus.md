# EventBus

The EventBus is simply another instance of Vue that we can import into any component/module, and either send to or receive events from one another without either knowing the other exists

### **Naming an event**

All you need to do is define and export it from [`event-bus.js`](../../../../web/src/services/event-bus.js)

```js
export const SEND_MESSAGE = 'send-message';
```

### **Emitting an event**

Import the event name and EventBus from [`event-bus.js`](../../../../web/src/services/event-bus.js)

```js
import { SEND_MESSAGE, EventBus } from '@/services/event-bus';
```

Call `$emit` on the EventBus passing the event name and desired payload(s) as arguments (depending on your receiver implementation)

```js
EventBus.$emit(SEND_MESSAGE, { sender: 'NHS App', message: 'Hello!' });
// or
EventBus.$emit(SEND_MESSAGE, 'NHS App', 'Hello!');
```

### **Receiving an event**

Again import the event name and EventBus as above

Create a callback that will handle the payload

```js
methods: {
  onSendMessage(payload) {
    doSomething(payload.sender);
    doSomethingElse(payload.message);
  },
},
```

Register the callback using the EventBus `$on` method, passing the event name and callback

```js
beforeMount() {
  EventBus.$on(SEND_MESSAGE, this.onSendMessage);
},
```

> When registering callbacks in a Vue component, be sure to deregister it in `beforeDestroy`

Deregister the callback using the EventBus `$off` method, passing the event name and callback

```js
beforeDestroy() {
  EventBus.$off(SEND_MESSAGE, this.onSendMessage);
},
```

This isn't currently something we do, however it's also possible to:

Deregister all listeners for a given event

```js
EventBus.$off(SEND_MESSAGE);
```

Deregister all listeners on the EventBus entirely

```js
EventBus.$off();
```

___

For further reading, see the [Vue Docs on Custom Events](https://vuejs.org/v2/guide/components-custom-events.html)