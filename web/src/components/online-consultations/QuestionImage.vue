<template>
  <div>
    <div v-if="error && errorText">
      <span v-for="singleError in errorText"
            :id="`${id}error`" :key="singleError" class="nhsuk-error-message">
        <span class="nhsuk-u-visually-hidden">{{ $t('generic.input.errors.messagePrefix') }}</span>
        {{ singleError }}
      </span>
    </div>
    <generic-image-input :id="id"
                         :name="name"
                         :source="source"
                         :required="required"
                         :error="error"
                         @input="onImageClicked($event)"/>
  </div>
</template>

<script>
import GenericImageInput from '@/components/widgets/GenericImageInput';
import { questionImageAnswerValid } from '@/lib/online-consultations/answer-validators';

export default {
  name: 'QuestionImage',
  components: {
    GenericImageInput,
  },
  props: {
    id: {
      type: String,
      default: undefined,
    },
    name: {
      type: String,
      default: undefined,
    },
    source: {
      type: String,
      default: undefined,
    },
    required: {
      type: Boolean,
      default: true,
    },
    error: {
      type: Boolean,
      default: false,
    },
    errorText: {
      type: Array,
      default: undefined,
    },
  },
  data() {
    return {
      clickPosition: undefined,
    };
  },
  watch: {
    clickPosition(to) {
      this.checkAndEmitIsValueValid(to);
      this.$emit('input', to);
    },
  },
  created() {
    this.checkAndEmitIsValueValid(this.clickPosition);
  },
  methods: {
    checkAndEmitIsValueValid(value) {
      this.$emit('validate', questionImageAnswerValid(value, this.required));
    },
    onImageClicked(event) {
      this.clickPosition = {
        x: event.offsetX,
        y: event.offsetY,
      };
    },
  },
};
</script>
