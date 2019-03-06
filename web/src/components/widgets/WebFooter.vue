<template>
  <footer :class="$style.footer" role="contentinfo">
    <div :class="$style['footer-content']">
      <ul>
        <li :class="$style.nhsLogo">
          <HomeLink ref="headerHomeLink" :index-path="indexPath" data-purpose="homeLink"/>
        </li>
        <li><a data-purpose="nhsSitesLink" href="https://www.nhs.uk/Pages/nhs-sites.aspx" target="_blank">NHS sites</a></li>
        <li><a data-purpose="aboutUsLink" href="https://www.nhs.uk/Pages/about-us.aspx" target="_blank">About us</a></li>
        <li><a data-purpose="contactUsLink" href="https://www.nhs.uk/Pages/contact-us.aspx" target="_blank">Contact us</a></li>
      </ul>
      <ul>
        <li><a data-purpose="siteMapLink" href="https://www.nhs.uk/choices/pages/sitemap.aspx" target="_blank">Sitemap</a></li>
        <li>
          <a data-purpose="accessibilityInformationLink" href="https://www.nhs.uk/aboutnhschoices/aboutnhschoices/accessibility/pages/accessibilitystatement.aspx" target="_blank">Accessibility</a>
        </li>
        <li><a data-purpose="policiesLink" href="https://www.nhs.uk/Pages/site-policies.aspx" target="_blank">Our policies</a></li>
        <li>© Crown Copyright</li>
      </ul>
    </div>
  </footer>
</template>


<script>
/* eslint-disable no-unused-vars */
import { executeHomeNavigationRule } from '@/lib/routes';
import HomeLink from './HomeLink';

export default {
  components: {
    HomeLink,
  },
  computed: {
    indexPath() {
      return executeHomeNavigationRule(this.$route.name);
    },
  },
};
</script>


<style module lang="scss" scoped>
  @import "../../style/colours";
  @import '../../style/screensizes';
  @import '../../style/textstyles';


  .footer {
    background-color: $nhs_blue;
    color: $white;
    height: auto;
    display: block;
    bottom: 0;
    left: 0;
    right: 0;
    z-index: 4;
    box-sizing: border-box;

    ul li,
    ol li {
      vertical-align: middle;
    }

    .footer-content {
      @include main-container-width;
      margin: 0 auto;
      padding-left: 0.5em;

      .nhsLogo {
        color: $white;
        display: inline-block;

        a.anchor-icon {
          color: $white;
          display: inline-block;
        }
      }

      ul {
        border-bottom: 1px solid rgba(255, 255, 255, 0.2);
        list-style-type: none;
        padding: 1.5em 0;

        li {
          a {
            font-family: $frutiger-light;
            text-decoration: underline;
            font-weight: normal;
            color: $white;
            line-height: 1.5em;
            padding: 0.5em;
            outline: none;

            &:visited,
            &:active {
              color: $white;
            }

            &:focus {
              box-shadow: inset 0 0 0 4px $focus_highlight;
            }

            &:hover {
              background: #003d78;
              box-shadow: none;
              color: $white;
              text-decoration: underline;
            }
          }
        }
      }
    }

    @include tablet-and-above {
      .footer-content {
        margin-left: 2em;
      }

      .footer-content ul li {
        display: inline-block;
        font-size: 1.0625em;
        padding-right: 1.5em;
      }
    }

    @include desktop {
      .footer-content {
        margin: 0 auto;
      }
    }
  }
</style>
