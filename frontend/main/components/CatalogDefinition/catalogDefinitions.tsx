import React from 'react';
import { NoSsr, Typography, Grid } from '@material-ui/core';
import SearchBox from '../../widgets/Searchbox';
import Pagination from 'react-js-pagination';
import ListContainer from '../../core/ListContainer';
import { Container } from '@material-ui/core';
import { Table } from '@material-ui/core';
import { TableHead } from '@material-ui/core';
import { TableBody } from '@material-ui/core';
import { TableRow } from '@material-ui/core';
import { TableCell } from '@material-ui/core';
import { Button } from '@material-ui/core';
import { Icon } from '@material-ui/core';
import Dialog from '../../widgets/Dialog';
import CatalogDefinition from './catalogDefinition';
import { AppBar, Toolbar } from '@material-ui/core';

import CatalogDefinitionService from './catalogdefinition.service';
import Router from 'next/router';

const service = new CatalogDefinitionService();
const defaultConfig = {
  service,
  filterName: 'FilterCatalogDefinition',
  sortName: 'SortCatalogDefinition'
};

interface CatalogDefinitionsProps {}

class CatalogDefinitionsList extends ListContainer<CatalogDefinitionsProps> {
  constructor(props, config) {
    super(props, Object.assign(defaultConfig, config));
  }

  componentDidMount() {
    this.load('OrderBy=Name');
  }

  AFTER_CREATE = instance => {
    this.openDialog('catalog', instance);
  };

  ON_OPEN_ITEM = item => {
    this.openDialog('catalog', item);
  };

  open = item => {
    Router.push(`/catalog?CatalogDefinitionId=${item.Id}`);
  };

  render() {
    const { isLoading, filterOptions, baseList } = this.state;
    return (
      <NoSsr>
        <Container style={{ padding: 20 }}>
          <Typography variant='h4' className='h4' gutterBottom>
            Catalogs
          </Typography>
          <Grid container direction='row'>
            <Grid item xs />
            <Pagination
              activePage={filterOptions.page}
              itemsCountPerPage={filterOptions.limit}
              totalItemsCount={filterOptions.itemsCount}
              pageRangeDisplayed={5}
              onChange={newPage => {
                this.pageChanged(newPage);
              }}
            />
          </Grid>
          <Table size='small'>
            <TableHead>
              <TableRow>
                <TableCell style={{ width: 260 }} />
                <TableCell style={{ width: 170 }}>Name</TableCell>
                <TableCell>Fields</TableCell>
                <TableCell>Relationships</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {baseList &&
                baseList.map(item => (
                  <TableRow key={item.Id}>
                    <TableCell>
                      <Grid container direction='row' className='row' justify='center' alignItems='center' spacing={2}>
                        <Grid item xs>
                          <Button
                            variant='contained'
                            color='default'
                            size='small'
                            onClick={event => {
                              this.openItem(event, item);
                            }}
                          >
                            <Icon>build</Icon> Definition
                          </Button>
                        </Grid>
                        <Grid item xs>
                          <Button
                            variant='contained'
                            color='default'
                            size='small'
                            onClick={() => {
                              this.open(item);
                            }}
                          >
                            <Icon>view_list</Icon> Data
                          </Button>
                        </Grid>
                      </Grid>
                    </TableCell>
                    <TableCell>{item.Name}</TableCell>
                    <TableCell>{item.ConvertedFields && item.ConvertedFields.map(e => e.FieldName).join(', ')}</TableCell>
                    <TableCell>
                      {item.ConvertedRelationships && item.ConvertedRelationships.map(e => e.FieldType + ': ' + e.ForeignValue).join(', ')}
                    </TableCell>
                  </TableRow>
                ))}
            </TableBody>
          </Table>
        </Container>
        <Dialog opener={this} id='catalog' title='Catalog Definition' okLabel='Save' maxWidth='md'>
          {dialog => {
            return !isLoading && <CatalogDefinition dialog={dialog} data={this.state.catalog} />;
          }}
        </Dialog>
        <AppBar position='fixed' style={{ top: 'auto', bottom: 0 }}>
          <Toolbar variant='dense'>
            <SearchBox
              bindFilterInput={this.bindFilterInput}
              value={filterOptions.filterGeneral}
              clear={() => this.clearInput('filterGeneral')}
            />
            <Grid item xs />
            <Button
              variant='contained'
              color='default'
              onClick={event => {
                this.createInstance({});
              }}
            >
              <Icon>add_circle</Icon>New
            </Button>
          </Toolbar>
        </AppBar>
      </NoSsr>
    );
  }
}

export default CatalogDefinitionsList;
