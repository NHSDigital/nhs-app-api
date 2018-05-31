<template>
  <div id="mainDiv">
    <div class="checkbox-panel">
      <input type="hidden">
      <label class="repeat-prescription-name">{{ prescriptionDetails.name }}</label>
    </div>
    <p>{{ prescriptionDetails.dosage }} - {{ prescriptionDetails.quantity }}</p>
  </div>
</template>
<script>
export default{
  name: 'RepeatPrescription',
  props: {
    prescriptionDetails: {
      type: Object,
      required: true,
    },
  },
};
</script>
