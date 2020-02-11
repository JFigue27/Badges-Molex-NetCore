import React, { Component } from 'react';
import { Grid, Typography } from '@material-ui/core';
export default class Loader extends Component {
  render() {
    return (
      <Grid container direction='column' alignItems='center' alignContent='center' item xs={12} style={{ padding: 20, paddingTop: '15%' }}>
        <Grid item>
          <div className='lds-grid'>
            <div />
            <div />
            <div />
            <div />
            <div />
            <div />
            <div />
            <div />
            <div />
          </div>
          <Typography>Loading ...</Typography>
        </Grid>
      </Grid>
    );
  }
}
