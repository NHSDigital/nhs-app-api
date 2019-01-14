<template>
  <form :class="className" :action="action" method="get">
    <input :name="inputName" :value="JSON.stringify(value)" type="hidden">
    <slot/>
  </form>
</template>

<script>
import { noJsParameterName } from '@/lib/noJs';

export default {
  props: {
    value: {
      type: Object,
      required: true,
    },
    action: {
      type: String,
      required: true,
    },
    className: {
      type: String,
      required: false,
      default: '',
    },
  },
  computed: {
    inputName() {
      return noJsParameterName;
    },
  },
};
</script>
