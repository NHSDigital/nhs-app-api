<template>
  <generic-button class="nhsuk-button nhsuk-button--secondary"
                  @click.stop.prevent="$emit('click', $event)">
    {{ $t(from) }}
  </generic-button>
</template>

<script>
import GenericButton from '@/components/widgets/GenericButton';

export default {
  name: 'ErrorButton',
  components: {
    GenericButton,
  },
  props: {
    from: {
      type: String,
      required: true,
    },
  },
};
</script>
