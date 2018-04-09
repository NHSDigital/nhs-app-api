<template>
  <div id="mainDiv">
    <main class="content">
      <div class="info">
        <h5>Welcome, James Doyle</h5>
        <p>Get medical advice, book GP appointments and order repeat prescriptions any time.</p>
      </div>
      <div class="panel">
        <div class="panel-icon">
          <patient-details-icon />
        </div>
        <div class="panel-content patient">
          <h5>James Doyle</h5>
          <hr>
          <div style="display: inline-block; float: left;">
            <label>Date of birth</label>
            <p>20th January 1982</p>
          </div>
          <div style="display: inline-block; float: left; margin-left: 28px;">
            <label>NHS number</label>
            <p>1234567890</p>
          </div>
        </div>
      </div>
    </main>
  </div>
</template>

<script>
import PatientDetailsIcon from '@/components/icons/PatientDetailsIcon';

export default {
  components: {
    PatientDetailsIcon,
  },
};
</script>

<style lang="scss">
  @import '../style/html';
  @import '../style/textstyles';
  @import '../style/elements';
  @import '../style/fonts';
  @import '../style/colours';

  #mainDiv {
    padding: 0;
    @include default_text;
  }

  .panel label,
  .form label {
    padding-top: 0px;
  }
</style>
