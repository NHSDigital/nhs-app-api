<template>
  <contents-list class="nhsuk-u-padding-top-6">
    <contents-list-item id="contents-overview"
                        :href="overviewPath"
                        :route-name="overviewName"
                        :text="$t('ds01.titles.p1')"
                        :click-param="overviewPath"
                        :click-func="goToUrl"/>

    <contents-list-item id="contents-confidentialPatientInfo"
                        :href="whereUsedPath"
                        :route-name="whereUsedName"
                        :text="$t('ds01.titles.p2')"
                        :click-param="whereUsedPath"
                        :click-func="goToUrl"/>

    <contents-list-item id="contents-choiceDoesNotApply"
                        :href="doesNotApplyPath"
                        :route-name="doesNotApplyName"
                        :text="$t('ds01.titles.p3')"
                        :click-param="doesNotApplyPath"
                        :click-func="goToUrl"/>

    <contents-list-item id="contents-makeChoice"
                        :href="makeYourChoicePath"
                        :route-name="makeYourChoiceName"
                        :text="$t('ds01.titles.p4')"
                        :click-param="makeYourChoicePath"
                        :click-func="goToUrl"/>
  </contents-list>
</template>

<script>
import ContentsList from '@/components/ContentsList';
import ContentsListItem from '@/components/ContentsListItem';
import {
  DATA_SHARING_DOES_NOT_APPLY_PATH,
  DATA_SHARING_MAKE_YOUR_CHOICE_PATH,
  DATA_SHARING_OVERVIEW_PATH,
  DATA_SHARING_WHERE_USED_PATH,
} from '@/router/paths';
import {
  DATA_SHARING_DOES_NOT_APPLY_NAME,
  DATA_SHARING_MAKE_YOUR_CHOICE_NAME,
  DATA_SHARING_OVERVIEW_NAME,
  DATA_SHARING_WHERE_USED_NAME,
} from '@/router/names';

export default {
  name: 'Contents',
  components: {
    ContentsList,
    ContentsListItem,
  },
  data() {
    return {
      doesNotApplyPath: DATA_SHARING_DOES_NOT_APPLY_PATH,
      makeYourChoicePath: DATA_SHARING_MAKE_YOUR_CHOICE_PATH,
      overviewPath: DATA_SHARING_OVERVIEW_PATH,
      whereUsedPath: DATA_SHARING_WHERE_USED_PATH,
      doesNotApplyName: DATA_SHARING_DOES_NOT_APPLY_NAME,
      makeYourChoiceName: DATA_SHARING_MAKE_YOUR_CHOICE_NAME,
      overviewName: DATA_SHARING_OVERVIEW_NAME,
      whereUsedName: DATA_SHARING_WHERE_USED_NAME,
    };
  },
};
</script>
