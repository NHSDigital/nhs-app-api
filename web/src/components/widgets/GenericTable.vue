<template>
  <table class="nhsuk-table">
    <caption v-if="caption" class="nhsuk-table__caption">{{ caption }}</caption>
    <thead v-if="headings" class="nhsuk-table__head">
      <tr class="nhsuk-table__row">
        <th v-for="heading in headings"
            :key="`heading-${heading}`"
            class="nhsuk-table__header"
            scope="col">
          {{ heading }}
        </th>
      </tr>
    </thead>
    <tbody v-if="rows && rows.length > 0" class="nhsuk-table__body">
      <tr v-for="(row, index) in rows" :key="`row-${index}`" class="nhsuk-table__row">
        <td v-for="(rowValue, rowIndex) in row"
            :key="`rowValue-${rowIndex}`"
            class="nhsuk-table__cell">
          {{ rowValue }}
        </td>
      </tr>
    </tbody>
    <tbody v-else class="nhsuk-table__body">
      <tr class="nhsuk-table__row">
        <td class="nhsuk-table__cell"
            :colspan="headings ? headings.length : 1">
          {{ $t('generic.table.errors.noData') }}
        </td>
      </tr>
    </tbody>
  </table>
</template>

<script>
export default {
  props: {
    caption: {
      type: String,
      default: undefined,
    },
    headings: {
      type: Array,
      required: true,
    },
    rows: {
      type: Array,
      required: true,
    },
  },
};
</script>

<style module lang="scss" scoped>
 @import "../../../node_modules/nhsuk-frontend/packages/core/tools/ifff";
 @import "../../../node_modules/nhsuk-frontend/packages/core/settings/typography";
 @import "../../../node_modules/nhsuk-frontend/packages/core/tools/typography";
 @import "../../../node_modules/nhsuk-frontend/packages/core/settings/colours";
 @import "../../../node_modules/nhsuk-frontend/packages/core/settings/globals";
 @import "../../../node_modules/nhsuk-frontend/packages/core/tools/sass-mq";
 @import "../../../node_modules/nhsuk-frontend/packages/core/settings/spacing";
 @import "../../../node_modules/nhsuk-frontend/packages/core/tools/spacing";
 @import "../../../node_modules/nhsuk-frontend/packages/core/tools/mixins";
 @import "../../../node_modules/nhsuk-frontend/packages/core/elements/table";
 @import "../../../node_modules/nhsuk-frontend/packages/core/utilities/visually-hidden";
 @import "../../../node_modules/nhsuk-frontend/packages/components/tables/tables";
</style>
