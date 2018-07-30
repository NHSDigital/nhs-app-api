<template>
  <nav class="menu">
    <ul>
      <li :class="getMenuitemState(0)">
        <a href="/symptoms"
           data-sid="symptoms-menu-item"
           @click="setMenuitemState($event)">
          <symptoms-icon/>
          <span>{{ $t('navigationMenu.symptomsLabel') }}</span>
        </a>
      </li>
      <li :class="getMenuitemState(1)">
        <a href="/appointments"
           data-sid="appointments-menu-item"
           @click="setMenuitemState($event)">
          <appointments-icon/>
          <span>{{ $t('navigationMenu.appointmentsLabel') }}</span>
        </a>
      </li>
      <li :class="getMenuitemState(2)">
        <a href="/prescriptions"
           data-sid="prescriptions-menu-item"
           @click="setMenuitemState($event)">
          <prescriptions-icon/>
          <span>{{ $t('navigationMenu.prescriptionsLabel') }}</span>
        </a>
      </li>
      <li :class="getMenuitemState(3)">
        <a href="/my-record/myrecordwarning"
           data-sid="myrecord-menu-item"
           @click="setMenuitemState($event)">
          <record-icon/>
          <span>{{ $t('navigationMenu.myRecordLabel') }}</span>
        </a>
      </li>
      <li :class="getMenuitemState(4)">
        <a href="/more"
           data-sid="more-menu-item"
           @click="setMenuitemState($event)">
          <more-icon/>
          <span>{{ $t('navigationMenu.moreLabel') }}</span>
        </a>
      </li>
    </ul>
  </nav>
</template>

<script>
/* eslint-disable import/extensions */
import SymptomsIcon from '@/components/icons/SymptomsIcon';
import AppointmentsIcon from '@/components/icons/AppointmentsIcon';
import PrescriptionsIcon from '@/components/icons/PrescriptionsIcon';
import RecordIcon from '@/components/icons/MyRecordIcon';
import MoreIcon from '@/components/icons/MoreIcon';

export default {
  components: {
    SymptomsIcon,
    AppointmentsIcon,
    PrescriptionsIcon,
    RecordIcon,
    MoreIcon,
  },
  methods: {
    getMenuitemState(menuItemIndex) {
      // eslint-disable-next-line
      return {
        active: this.$store.state.navigation.menuItemStatusAt[menuItemIndex],
      };
    },
    setMenuitemState(event) {
      const a = event.currentTarget;
      if (a.target === '_blank') {
        window.open(a.href, '_blank');
      } else {
        this.$router.push(a.pathname);
      }

      event.preventDefault();
    },
  },
};
</script>

<style lang="scss" scoped>
  @import '../style/colours';
  @import '../style/fonts';
  @import '../style/textstyles';

  nav.menu {
    background-color: $white;
    position: fixed;
    bottom: 0px;
    left: 0px;
    right: 0px;
    height: 70px;
    box-shadow: 0px 0px 5px rgba(0, 0, 0, .5);
    z-index: 1000;

    ul {
      width: 100%;
      box-sizing: border-box;
      display: flex;
      flex-direction: row;
      flex-wrap: wrap;
      padding-left: 6px;
      padding-right: 6px;
      margin: 0;


      li {
        flex-grow: 1;
        text-align: center;
        vertical-align: bottom;
        list-style: none;
        margin-top: 4px;
        align-self: stretch;
        justify-content: space-around;
        display: flex;
        flex-direction: column;

        a {
          text-decoration: none;
          display: inline-block;
          text-align: center;
          flex-grow: 1;
          justify-content: space-around;
        }

        svg {
          display: block;
          align-self: center;
          flex-grow: 1;
          height: 30px;
          margin: 6px auto 3px;
        }

        span {
          align-self: flex-end;
          box-sizing: border-box;
          color: $dark_grey;
          display: inline-block;
          font-family: $default;
          font-weight: normal;
          font-size: 10px;
          margin-top: 6px;
          text-align: center;
          text-transform: uppercase;
          line-height: 10px;
          width: 100%;
        }
      }

      li.active {
        span {
          font-family: $default;
          font-weight: normal;
          font-size: 10px;
          text-transform: uppercase;
          color: $nhs_blue;
        }
      }
    }
  }

  header {
    background: $nhs_blue;
    position: fixed;
    top: 0px;
    left: 0px;
    right: 0px;
    height: 100px;
    box-shadow: 0px 0px 5px rgba(0, 0, 0, .5);

    h1 {
      @include screen_title;
      text-align: center;
    }

    .nhs_logo {
      margin: 16px;
      height: 22px;
      float: left;
    }

    .account {
      margin: 16px;
      float: right;
      height: 22px;
    }

    hr {
      margin: 54px 16px 9px 16px;
      height: 1px;
      border: none;
      background-color: white;
      opacity: 0.4;
    }
  }

</style>
