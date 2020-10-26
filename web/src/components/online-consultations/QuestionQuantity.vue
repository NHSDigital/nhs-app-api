<template>
  <div>
    <div v-if="error && errorText">
      <span v-for="singleError in errorText"
            :id="`${name}error`" :key="singleError" class="nhsuk-error-message">
        <span class="nhsuk-u-visually-hidden">{{ $t('generic.errorPrefix') }}</span>
        {{ singleError }}
      </span>
    </div>
    <div class="nhsuk-input__item">
      <label :id="`${name}-quantity-label`"
             :for="`${name}-quantity`"
             class="nhsuk-label nhsuk-input__label">
        {{ $t('generic.quantity') }}
      </label>
      <input :id="`${name}-quantity`"
             v-model="quantity"
             :class="['nhsuk-input nhsuk-input--width-2',
                      'ios-accessibility',
                      error && 'nhsuk-input--error']"
             type="number"
             :name="`${name}-quantity`"
             pattern="[0-9]+"
             :min="min"
             :max="maxValue"
             :required="required">
    </div>

    <div class="nhsuk-input__item">
      <label :id="`${name}-select-label`"
             :for="`${name}-unit`"
             class="nhsuk-label nhsuk-input__label">
        {{ $t('generic.unit') }}
      </label>
      <select-dropdown v-model="unit"
                       :select-id="`${name}-unit`"
                       :select-name="`${name}-unit`"
                       :error-border="error"
                       :required="required">
        <option disabled="" selected="" value="">
          {{ $t('generic.selectUnit') }}
        </option>
        <option v-for="option in options"
                :key="option.code"
                :value="option.code"
                :selected="option.code === quantityAnswer.unit">
          {{ option.label }}
        </option>
      </select-dropdown>
    </div>
  </div>
</template>

<script>
import SelectDropdown from '@/components/widgets/SelectDropdown';
import { questionQuantityAnswerValid } from '@/lib/online-consultations/answer-validators';

export default {
  name: 'QuestionQuantity',
  components: {
    SelectDropdown,
  },
  props: {
    name: {
      type: String,
      required: true,
    },
    options: {
      type: Array,
      required: true,
    },
    min: {
      type: Number,
      default: undefined,
    },
    maxValue: {
      type: Number,
      default: undefined,
    },
    value: {
      type: Object,
      default() {
        return {
          unit: '',
          quantity: '',
        };
      },
    },
    error: {
      type: Boolean,
      default: false,
    },
    errorText: {
      type: Array,
      default: undefined,
    },
    required: {
      type: Boolean,
      default: true,
    },
  },
  data() {
    return {
      quantityAnswer: this.value,
    };
  },
  computed: {
    validValues() {
      return this.options.map(o => o.code);
    },
    quantity: {
      get() {
        return this.quantityAnswer.quantity;
      },
      set(quantity) {
        this.quantityAnswer = { ...this.quantityAnswer, quantity };
      },
    },
    unit: {
      get() {
        return this.quantityAnswer.unit;
      },
      set(unit) {
        this.quantityAnswer = { ...this.quantityAnswer, unit };
      },
    },
  },
  watch: {
    quantityAnswer(to) {
      this.checkAndEmitIsValueValid(to);
      this.$emit('input', to);
    },
  },
  created() {
    this.checkAndEmitIsValueValid(this.quantityAnswer);
  },
  methods: {
    checkAndEmitIsValueValid(value) {
      this.$emit('validate', questionQuantityAnswerValid(value, this.required, this.min, this.maxValue, this.validValues));
    },
  },
};
</script>
<style lang="scss" scoped>
  .ios-accessibility {
      min-height: 40px !important;
      height: auto !important;
  }
</style>
