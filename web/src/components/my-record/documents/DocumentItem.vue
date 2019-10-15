<template>
  <card :id="`document-${id}`"
        :component="documentTag"
        :class="['document', documentAvailable && 'available']"
        :href="documentPath + documentQuery"
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
import { MY_RECORD_DOCUMENT } from '@/lib/routes';

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
  },
  data() {
    const documentAvailable = this.available && this.id;
    const noJsData = JSON.stringify({ myRecord: { hasAcceptedTerms: true } });
    return {
      documentQuery: `?nojs=${encodeURIComponent(noJsData)}` +
        `&type=${encodeURIComponent(this.type)}` +
        `&name=${encodeURIComponent(this.name)}`,
      documentAvailable,
      documentTag: documentAvailable ? 'a' : 'div',
      documentPath: MY_RECORD_DOCUMENT.path.replace(':id', this.id),
    };
  },
  methods: {
    documentClicked() {
      if (this.documentAvailable) {
        this.$store.dispatch('myRecord/setSelectedDocumentInfo', {
          type: this.type,
          name: this.name,
        });
        this.$router.push({ name: MY_RECORD_DOCUMENT.name, params: { id: this.id } });
      }
    },
  },
};
</script>

<style lang="scss">
  .document {
    &.available {
      padding-right: 2em;
      background-image: url(~assets/icon_arrow_left.svg);
      background-position: right 1em center;
      background-repeat: no-repeat;
    }

    .document__date-and-type {
      font-size: 18px;
      font-weight: bold;
    }

    .document__name,
    .document__term {
      font-size: 16px;
      font-weight: normal;
    }

    .document__name,
    .document__term {
      margin-top: 5px;
    }
  }
</style>
