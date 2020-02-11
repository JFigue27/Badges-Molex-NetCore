import React from 'react';
import Head from 'next/head';
import Link from 'next/link';
import { withRouter } from 'next/router';
import { Drawer, AppBar, Toolbar, IconButton, Typography, Menu, MenuItem, Grid, Tabs, Tab } from '@material-ui/core';
import { Button, Icon, Fab, ButtonBase } from '@material-ui/core';
import Dialog from '../../widgets/Dialog';
import AuthService from '../../core/AuthService';
import Login from '../../widgets/Login';
import '../../styles.scss';
import { MuiPickersUtilsProvider } from '@material-ui/pickers';
import MomentUtils from '@date-io/moment';
import { SnackbarProvider } from 'notistack';
import { GlobalContext } from './globals-context';
import { IAuth } from '../../core/Contract';

interface IIndexState {
  auth?: IAuth;
  anchorEl?: any;
  currentTab: boolean;
  loginOpen: boolean;
  loading: boolean;
  globals: any;
}

class App extends React.Component {
  pages = [
    ///start:slot:pages<<<
    ///end:slot:pages<<<
  ];

  state: IIndexState = {
    auth: undefined,
    anchorEl: null,
    currentTab: false,
    loginOpen: false,
    loading: true,
    globals: {}
  };

  classes = {
    root: {
      flexGrow: 1
    },
    grow: {
      flexGrow: 1
    },
    menuButton: {
      marginLeft: -12,
      marginRight: 20
    }
  };

  componentDidMount() {
    AuthService.fillAuthData();
    AuthService.OpenLogin = this.openLoginDialog;
    if (!AuthService.auth) {
      AuthService.OpenLogin();
    }

    let currentPath = window.location.pathname;
    let findRoute = this.pages.findIndex(e => (e as any).href.toLowerCase() == currentPath.toLowerCase());

    this.setState({
      loading: false,
      auth: AuthService.auth,
      currentTab: findRoute > -1 ? findRoute : false,
      globals: { auth: AuthService.auth }
    });
  }
  openLoginDialog = () => {
    this.setState({ loginOpen: true });
  };
  closeLoginDialog = () => {
    this.setState({ loginOpen: false, auth: AuthService.auth, globals: { auth: AuthService.auth } });
  };

  logout = () => {
    AuthService.logout().then(() => {
      this.setState({ auth: AuthService.auth, globals: { auth: AuthService.auth } });
    });
    this.openLoginDialog();
  };

  toggleDrawer = (side: string, open: boolean) => () => {
    this.setState({
      [side]: open
    });
  };
  handleMenu = (event: any) => {
    this.setState({ anchorEl: event.currentTarget });
  };
  handleClose = () => {
    this.setState({ anchorEl: null });
  };

  handleTabsChange = (event: any, newValue: number) => {
    this.setState({ currentTab: newValue });
  };

  render() {
    let classes = this.classes as any;
    const { auth, anchorEl } = this.state;
    const open = Boolean(anchorEl);
    const fullscreen = !!(this.props as any).router.query.hasOwnProperty('fullscreen');

    function LinkTab(props: any) {
      let { href } = props;
      return (
        <Link href={href} passHref>
          <Tab component='a' {...props} />
        </Link>
      );
    }

    return (
      <div style={{ visibility: this.state.loading ? 'hidden' : 'visible' }}>
        <style global jsx>
          {`
            body {
              margin: 0;
              padding-top: ${fullscreen ? 0 : '50px'};
            }
          `}
        </style>
        <Head>
          <title>Main</title>
          <meta charSet='utf-8' />
          <meta name='viewport' content='width=device-width, initial-scale=1.0, minimum-scale=1.0, maximum-scale=1.0, user-scalable=no' />
          <meta name='format-detection' content='telephone=no' />
          <meta name='msapplication-tap-highlight' content='no' />
          <meta name='apple-mobile-web-app-capable' content='yes' />
          <meta name='apple-mobile-web-app-status-bar-style' content='black' />

          <link rel='stylesheet' href='https://fonts.googleapis.com/css?family=Roboto:300,400,500' />
          <link rel='stylesheet' href='https://fonts.googleapis.com/icon?family=Material+Icons' />
          <link rel='icon' type='image/x-icon' href='/static/favicon.ico' />
        </Head>
        <Dialog open={this.state.loginOpen} onClose={this.closeLoginDialog} fullScreen actionsOff>
          {() => <Login onCloseLogin={this.closeLoginDialog} />}
        </Dialog>
        {!fullscreen && (
          <AppBar position='fixed' className='MainAppBar app-nav'>
            <Toolbar>
              {/* <IconButton color='inherit' onClick={this.toggleDrawer('right', true)}>
              <Icon>menu</Icon>
            </IconButton> */}
              <Link href={'/'}>
                <ButtonBase>
                  <Typography variant='h6' color='inherit'>
                    Main
                  </Typography>
                </ButtonBase>
              </Link>
              <Grid item xs />

              <Tabs variant='scrollable' value={this.state.currentTab} onChange={this.handleTabsChange}>
                {this.pages.map((page: any, index: number) => {
                  return <LinkTab key={index} label={page.label} href={page.href} />;
                })}
              </Tabs>
              <Button color='inherit' className={classes.button} onClick={this.handleMenu}>
                <Icon style={{ marginRight: 5 }}>account_circle</Icon>
                {auth && auth.user && auth.user.UserName}
              </Button>
              <Menu
                id='menu-appbar'
                anchorEl={anchorEl}
                anchorOrigin={{
                  vertical: 'top',
                  horizontal: 'right'
                }}
                transformOrigin={{
                  vertical: 'top',
                  horizontal: 'right'
                }}
                open={open}
                onClose={this.handleClose}
              >
                {/* <MenuItem onClick={this.handleClose}>Profile</MenuItem> */}
                <MenuItem onClick={this.logout}>Logout</MenuItem>
              </Menu>
            </Toolbar>
          </AppBar>
        )}
        {/* <Drawer anchor='left' open={this.state.right} onClose={this.toggleDrawer('right', false)}>
          <div tabIndex={0} role='button' onClick={this.toggleDrawer('right', false)} onKeyDown={this.toggleDrawer('right', false)}>
            <div style={{ width: 200 }}>Content</div>
          </div>
        </Drawer> */}
        <SnackbarProvider autoHideDuration={700} anchorOrigin={{ vertical: 'top', horizontal: 'right' }} maxSnack={4}>
          <MuiPickersUtilsProvider utils={MomentUtils}>
            <GlobalContext.Provider value={this.state.globals}>
              <Grid container direction='column' item xs={12} style={{ paddingTop: '2%' }}>
                {this.props.children}
              </Grid>
            </GlobalContext.Provider>
          </MuiPickersUtilsProvider>
        </SnackbarProvider>
      </div>
    );
  }
}

export default withRouter(App as any);
