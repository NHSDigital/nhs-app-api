<template>
  <footer :class="$style.footer" role="contentinfo">
    <div :class="$style['footer-content']">
      <ul>
        <li :class="$style.nhsLogo">
          <HomeLink ref="headerHomeLink" :index-path="indexPath"/>
        </li>
        <li><a href="https://www.nhs.uk/Pages/nhs-sites.aspx" target="_blank">NHS sites</a></li>
        <li><a href="https://www.nhs.uk/Pages/about-us.aspx" target="_blank">About us</a></li>
        <li><a href="https://www.nhs.uk/Pages/contact-us.aspx" target="_blank">Contact us</a></li>
      </ul>
      <ul>
        <li><a href="https://www.nhs.uk/choices/pages/sitemap.aspx" target="_blank">Sitemap</a></li>
        <li>
          <a href="https://www.nhs.uk/aboutnhschoices/aboutnhschoices/accessibility/pages/accessibilitystatement.aspx" target="_blank">Accessibility</a>
        </li>
        <li><a href="https://www.nhs.uk/Pages/site-policies.aspx" target="_blank">Our policies</a></li>
        <li>© Crown Copyright</li>
      </ul>
    </div>
  </footer>
</template>


<script>
/* eslint-disable no-unused-vars */
import { INDEX } from '@/lib/routes';
import HomeLink from './HomeLink';

export default {
  components: {
    HomeLink,
  },
  data() {
    return {
      indexPath: INDEX.path,
    };
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
      /*margin: 0;*/
      vertical-align: middle;
    }

    .footer-content {
      max-width: 960px;
      margin: 0 auto;
      padding-left: 1em;

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

    @media (min-width: 40.0625em) {
      .footer-content {
        margin-left: 32px;
      }

      .footer-content ul li {
        display: inline-block;
        font-size: 17px;
        padding-right: 25px;
      }
    }

    @media (min-width: 1024px) {
      .footer-content {
        margin: 0 auto;
      }
    }
  }
</style>
