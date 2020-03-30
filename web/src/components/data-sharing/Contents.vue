<template>
  <contents-list class="nhsuk-u-padding-top-6">
    <contents-list-item id="contents-overview"
                        :href="overviewPath"
                        :text="$t('ds01.titles.p1')"
                        :click-param="overviewPath"
                        :click-func="goToUrl"/>

    <contents-list-item id="contents-confidentialPatientInfo"
                        :href="whereUsedPath"
                        :text="$t('ds01.titles.p2')"
                        :click-param="whereUsedPath"
                        :click-func="goToUrl"/>

    <contents-list-item id="contents-choiceDoesNotApply"
                        :href="doesNotApplyPath"
                        :text="$t('ds01.titles.p3')"
                        :click-param="doesNotApplyPath"
                        :click-func="goToUrl"/>

    <contents-list-item id="contents-makeChoice"
                        :href="makeYourChoicePath"
                        :text="$t('ds01.titles.p4')"
                        :click-param="makeYourChoicePath"
                        :click-func="goToUrl"/>
  </contents-list>
</template>

<script>
import ContentsList from '@/components/ContentsList';
import ContentsListItem from '@/components/ContentsListItem';
import {
  DATA_SHARING_DOES_NOT_APPLY,
  DATA_SHARING_MAKE_YOUR_CHOICE,
  DATA_SHARING_OVERVIEW,
  DATA_SHARING_WHERE_USED,
} from '@/lib/routes';

export default {
  name: 'Contents',
  components: {
    ContentsList,
    ContentsListItem,
  },
  data() {
    return {
      doesNotApplyPath: DATA_SHARING_DOES_NOT_APPLY.path,
      makeYourChoicePath: DATA_SHARING_MAKE_YOUR_CHOICE.path,
      overviewPath: DATA_SHARING_OVERVIEW.path,
      whereUsedPath: DATA_SHARING_WHERE_USED.path,
    };
  },
};
</script>
