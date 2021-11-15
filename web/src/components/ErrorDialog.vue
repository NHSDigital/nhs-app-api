<template>
  <div role="alert" aria-atomic="true">
    <message-dialog message-type="error" :focusable="true">
      <message-text data-purpose="error-heading">
        {{ $t(headerLocaleRef) }}
      </message-text>
      <message-list data-purpose="reason-error">
        <li v-for="error in validationErrors" :key="error">{{ error }}</li>
      </message-list>
    </message-dialog>
  </div>
</template>

<script>
import isArray from 'lodash/fp/isArray';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageList from '@/components/widgets/MessageList';
import MessageText from '@/components/widgets/MessageText';

export default {
  name: 'ErrorDialog',
  components: {
    MessageDialog,
    MessageList,
    MessageText,
  },
  props: {
    headerLocaleRef: {
      type: String,
      default: 'generic.thereIsAProblem',
    },
    errors: {
      type: [String, Array],
      required: true,
    },
  },
  computed: {
    validationErrors() {
      return isArray(this.errors) ? this.errors : [this.errors];
    },
  },
};
</script>
