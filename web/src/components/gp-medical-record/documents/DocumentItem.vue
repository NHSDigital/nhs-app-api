<template>
  <card :id="`document-${id}`"
        tabindex="0"
        component="a"
        class="document"
        @click.prevent="documentClicked">
    <p class="document__date-and-type">
      {{ date.value | datePart('YearMonthDay') }}
      ({{ type | uppercase }}, {{ sizeInBytes | readableBytes }})
    </p>
    <p v-if="term" class="document__term">
      {{ term }}
    </p>
    <p v-if="name" class="document__name">
      {{ name }}
    </p>
  </card>
</template>

<script>
import Card from '@/components/widgets/card/Card';
import { DOCUMENT } from '@/lib/routes';

export default {
  name: 'DocumentItem',
  components: {
    Card,
  },
  props: {
    available: {
      type: Boolean,
      default: false,
    },
    date: {
      type: Object,
      required: true,
    },
    type: {
      type: String,
      required: true,
    },
    sizeInBytes: {
      type: Number,
      required: true,
    },
    id: {
      type: String,
      required: true,
    },
    name: {
      type: String,
      default: undefined,
    },
    term: {
      type: String,
      default: undefined,
    },
    eventGuid: {
      type: String,
      default: undefined,
    },
    codeId: {
      type: Number,
      default: undefined,
    },
  },
  methods: {
    documentClicked() {
      this.$store.dispatch('myRecord/setSelectedDocumentInfo', {
        type: this.type,
        name: this.name,
        date: this.date,
        codeId: this.codeId,
        term: this.term,
        eventGuid: this.eventGuid,
        size: this.sizeInBytes,
      });
      this.$router.push({ name: DOCUMENT.name, params: { id: this.id } });
    },
  },
};
</script>

<style lang="scss">
  .document {
      padding-right: 2em;
      background-repeat: no-repeat;
      background-image: url(~assets/icon_arrow_left.svg);
      background-position: right 1em center;

    .document__name,
    .document__term {
      margin-top: 5px;
    }
  }
</style>
