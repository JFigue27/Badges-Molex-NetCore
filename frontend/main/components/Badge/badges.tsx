import React from 'react';
import { withRouter } from 'next/router';
import { withSnackbar } from 'notistack';
import { NoSsr, Typography, Grid } from '@material-ui/core';
import SearchBox from '../../widgets/Searchbox';
import Pagination from 'react-js-pagination';
import ListContainer, { ListProps } from '../../core/ListContainer';

import BadgeService from './badge.service';

const service = new BadgeService();
const defaultConfig = {
  service,
  filterName: 'Badges',
  sortname: 'Badges'
};

interface BadgeProps extends ListProps {}

class BadgesList extends ListContainer<BadgeProps> {
  constructor(props, config) {
    super(props, Object.assign(defaultConfig, config));
  }

  render() {
    const { isLoading, baseEntity, baseList, filterOptions, isDisabled } = this.state;

    return (
      <NoSsr>
        <Typography variant='h3' gutterBottom>
          This is the [badges] component.
        </Typography>
      </NoSsr>
    );
  }
}

export default withSnackbar(withRouter(BadgesList) as any) as React.ComponentClass<BadgeProps>;
