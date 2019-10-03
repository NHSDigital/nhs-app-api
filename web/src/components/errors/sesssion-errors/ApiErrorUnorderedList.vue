<template>
  <div v-if="list !== ''">
    <ul >
      <li v-for="(item, index) in list" :id="item.id"
          :key="index"
          :aria-label="item.label">
        {{ item.text }}
      </li>
    </ul>
  </div>
</template>

<script>
import ErrorMessageMixin from '@/components/errors/ErrorMessageMixin';
import isObject from 'lodash/fp/isObject';

export default {
  name: 'ApiErrorUnorderedList',
  mixins: [ErrorMessageMixin],
  props: {
    from: {
      type: String,
      default: '',
    },
  },
  computed: {
    overrideStyle() {
      return this.$store.state.errors.pageSettings.errorOverrideStyles[this.statusCode];
    },
    list() {
      const listValue = this.from !== '' ? this.getText(this.from) : '';
      if (!isObject(listValue)) {
        return '';
      }
      return listValue;
    },

  },
};
</script>

<style module lang="scss" scoped>
@import "../../../style/listmenu";
ul li {
  margin-left: 1em;
}
</style>
