<template>
  <ul v-if="list">
    <li v-for="(item, index) in list" :id="item.id"
        :key="index"
        :aria-label="item.label">
      {{ item.text }}
    </li>
  </ul>
</template>

<script>
import isObject from 'lodash/fp/isObject';

export default {
  name: 'ErrorUnorderedList',
  props: {
    from: {
      type: String,
      required: true,
    },
  },
  computed: {
    list() {
      const listValue = this.$t(this.from);
      return isObject(listValue) ? listValue : null;
    },
  },
};
</script>
