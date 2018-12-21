<template>
  <div class="content">

    <p v-for="paragraph of $t('ds01.pages.p2.intro.paragraphs')" :key="paragraph">
      {{ paragraph }}
    </p>
    <ul>
      <li v-for="listItem of $t('ds01.pages.p2.intro.listItems')" :key="listItem">
        {{ listItem }}
      </li>
    </ul>

    <h2 :class="$style['title']">{{ $t('ds01.pages.p2.thoseWhoCant.title') }}</h2>
    <p>
      {{ $t('ds01.pages.p2.thoseWhoCant.paragraphs[0].part1') }}
      <strong>{{ $t('ds01.pages.p2.thoseWhoCant.paragraphs[0].emphasised') }}</strong>
      {{ $t('ds01.pages.p2.thoseWhoCant.paragraphs[0].part3') }}
    </p>
    <ul>
      <li v-for="listItem of $t('ds01.pages.p2.thoseWhoCant.listItems')" :key="listItem">
        {{ listItem }}
      </li>
    </ul>
    <p>{{ $t('ds01.pages.p2.thoseWhoCant.paragraph') }}</p>

    <h2 :class="$style['title']">{{ $t('ds01.pages.p2.dataProtection.title') }}</h2>
    <p v-for="paragraph of $t('ds01.pages.p2.dataProtection.paragraphs')" :key="paragraph">
      {{ paragraph }}
    </p>
    <ul>
      <li v-for="listItem of $t('ds01.pages.p2.dataProtection.listItems')" :key="listItem">
        {{ listItem }}
      </li>
    </ul>
    <p>{{ $t('ds01.pages.p2.dataProtection.paragraph') }}</p>

  </div>
</template>

<script>
export default {};
</script>

<style module scoped lang="scss">
@import '../../style/datasharingpage';
</style>
