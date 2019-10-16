<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <div v-if="showTemplate" :class="[$style.content,
                                        'pull-content',
                                        !$store.state.device.isNativeApp && $style.desktopWeb]">
        <div id="documentInfo" :class="$style.info" data-purpose="info">
          <p v-if="name">{{ dateString }}</p>
        </div>
        <menu-item-list data-sid="action-list-menu">
          <menu-item id="btn_viewDocument"
                     :text="$t('my_record.documents.actions.view')"
                     :aria-label="$t('my_record.documents.actions.view')"
                     :click-func="navigateToView"/>
        </menu-item-list>
      </div>
    </div>
  </div>
</template>
<script>
import get from 'lodash/fp/get';
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import { MY_RECORD_DOCUMENT_DETAIL, MYRECORD } from '@/lib/routes';
import hasAgreedToMedicalWarning from '@/lib/sessionStorage';
import { isFalsy, datePart } from '@/lib/utils';
import NativeApp from '@/services/native-app';

export default {
  layout: 'nhsuk-layout',
  components: {
    MenuItem,
    MenuItemList,
  },
  computed: {
    isNativeApp() {
      return this.$store.state.device.isNativeApp;
    },
  },
  asyncData({ store, redirect }) {
    const date = get('state.myRecord.document.date.value', store);

    if (isFalsy(store.app.$env.MY_RECORD_DOCUMENTS_ENABLED)
      || (!store.state.myRecord.hasAcceptedTerms && !hasAgreedToMedicalWarning()) || !date
    ) {
      redirect(MYRECORD.path);
      return {};
    }

    const name = get('state.myRecord.document.name', store);
    const dateString = `${store.app.i18n.t('my_record.documents.documentPageSubtext')} ${datePart(date, 'YearMonthDay')}`;

    if (name) {
      store.dispatch('header/updateHeaderText', name);
    } else {
      store.dispatch('header/updateHeaderText', dateString);
    }

    return {
      document: store.state.myRecord.document,
      dateString,
      name,
    };
  },
  mounted() {
    if (this.isNativeApp) {
      NativeApp.resetPageFocus();
    }
  },
  methods: {
    navigateToView() {
      this.$router.push({ name: MY_RECORD_DOCUMENT_DETAIL.name,
        params: { id: this.$route.params.id } });
    },
  },
};
</script>
<style module lang="scss" scoped>
  @import '../../../style/spacings';
  @import '../../../style/textstyles';

  .info {
    font-size: 1em;
    margin-bottom: 1em;
    margin-top: -1em;

    p {
      font-family: $default_web;
      font-weight: normal;
    }
}
</style>
