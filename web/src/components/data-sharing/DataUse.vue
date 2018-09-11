<template>
  <div class="content">

    <p v-for="paragraph of $t('ds01.pages.p3.intro.paragraphs')" :key="paragraph">
      {{ paragraph }}
    </p>
    <ul>
      <li v-for="listItem of $t('ds01.pages.p3.intro.listItems')" :key="listItem">
        {{ listItem }}
      </li>
    </ul>

    <h2 :class="$style['title']">{{ $t('ds01.pages.p3.thoseWhoCant.title') }}</h2>
    <p>{{ $t('ds01.pages.p3.thoseWhoCant.paragraphs[0]') }}</p>
    <p>
      {{ $t('ds01.pages.p3.thoseWhoCant.paragraphs[1].part1') }}
      <strong>{{ $t('ds01.pages.p3.thoseWhoCant.paragraphs[1].emphasised') }}</strong>
      {{ $t('ds01.pages.p3.thoseWhoCant.paragraphs[1].part3') }}
    </p>
    <ul>
      <li v-for="listItem of $t('ds01.pages.p3.thoseWhoCant.listItems')" :key="listItem">
        {{ listItem }}
      </li>
    </ul>
    <p>{{ $t('ds01.pages.p3.thoseWhoCant.paragraph') }}</p>

    <h2 :class="$style['title']">{{ $t('ds01.pages.p3.dataProtection.title') }}</h2>
    <p v-for="paragraph of $t('ds01.pages.p3.dataProtection.paragraphs')" :key="paragraph">
      {{ paragraph }}
    </p>
    <ul>
      <li v-for="listItem of $t('ds01.pages.p3.dataProtection.listItems')" :key="listItem">
        {{ listItem }}
      </li>
    </ul>
    <p>{{ $t('ds01.pages.p3.dataProtection.paragraph') }}</p>

  </div>
</template>

<script>
export default {};
</script>

<style module scoped lang="scss">
@import '../../style/datasharingpage';
</style>
