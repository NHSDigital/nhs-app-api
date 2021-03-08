<template>
  <fieldset class="nhsuk-fieldset">
    <div v-if="error && errorText">
      <span v-for="singleError in errorText"
            :id="`${name}error`" :key="singleError" class="nhsuk-error-message">
        <span class="nhsuk-u-visually-hidden">{{ $t('generic.errorPrefix') }}</span>
        {{ singleError }}
      </span>
    </div>
    <generic-radio-button :key="`${name}-true`"
                          :selected-value="selectedValue"
                          :label="$t('generic.yes')"
                          :name="name"
                          :value="'true'"
                          :required="required"
                          :a-described-by="ariaDescribed"
                          @select="selected"/>
    <generic-radio-button :key="`${name}-false`"
                          :selected-value="selectedValue"
                          :label="$t('generic.no')"
                          :name="name"
                          :value="'false'"
                          :required="required"
                          :a-described-by="ariaDescribed"
                          @select="selected"/>
  </fieldset>
</template>

<script>
import GenericRadioButton from '@/components/widgets/GenericRadioButton';
import { questionBooleanAnswerValid } from '@/lib/online-consultations/answer-validators';

export default {
  name: 'QuestionBoolean',
  components: {
    GenericRadioButton,
  },
  props: {
    name: {
      type: String,
      required: true,
    },
    error: {
      type: Boolean,
      default: false,
    },
    errorId: {
      type: String,
      default: undefined,
    },
    errorText: {
      type: Array,
      default: undefined,
    },
    value: {
      type: String,
      default: undefined,
      validator: value => (['true', 'false', undefined].includes(value)),
    },
    required: {
      type: Boolean,
      default: true,
    },
  },
  data() {
    return {
      selectedValue: this.value,
    };
  },
  computed: {
    ariaDescribed() {
      const ariaDescribedContent = [
        this.error && this.errorText ? `${this.name}error` : undefined,
        this.required ? undefined : 'optional-label',
      ].join(' ').trim();
      return ariaDescribedContent || undefined;
    },
  },
  watch: {
    selectedValue(to) {
      this.checkAndEmitIsValueValid(to);
      this.$emit('input', to);
    },
  },
  created() {
    this.checkAndEmitIsValueValid(this.selectedValue);
  },
  methods: {
    checkAndEmitIsValueValid(value) {
      this.$emit('validate', questionBooleanAnswerValid(value, this.required));
    },
    selected(value) {
      this.selectedValue = value;
    },
  },
};
</script>
