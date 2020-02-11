import React from 'react';
import { withRouter } from 'next/router';
import { NoSsr, Typography, Grid, Paper } from '@material-ui/core';
import SearchBox from '../../widgets/Searchbox';
import Pagination from 'react-js-pagination';
import ListContainer, { ListProps } from '../../core/ListContainer';
import { Container } from '@material-ui/core';
import { Table } from '@material-ui/core';
import { TableHead } from '@material-ui/core';
import { TableBody } from '@material-ui/core';
import { TableRow } from '@material-ui/core';
import { TableCell } from '@material-ui/core';
import { Button, IconButton } from '@material-ui/core';
import { Icon } from '@material-ui/core';

import Dialog from '../../widgets/Dialog';
import Catalog from './catalog';
import { AppBar, Toolbar } from '@material-ui/core';

import CatalogService from './catalog.service';
import CatalogDefinitionService from '../CatalogDefinition/catalogdefinition.service';
import { WithRouterProps } from 'next/dist/client/with-router';
const catalogDefinitionService = new CatalogDefinitionService();

const service = new CatalogService();
const defaultConfig = {
  service,
  filterName: 'Catalog'
};

interface CatalogsProps extends ListProps {}

class CatalogsList extends ListContainer<CatalogsProps> {
  constructor(props, config) {
    super(props, Object.assign(defaultConfig, config));
  }

  componentDidMount() {
    let catalogDefinitionId = this.getParameterByName('CatalogDefinitionId');
    if (!catalogDefinitionId) return;
    this.load('CatalogDefinitionId=' + catalogDefinitionId);
    catalogDefinitionService.LoadEntity(catalogDefinitionId).then(definition => {
      this.setState({ definition });
      let foreigns = {};
      definition.ConvertedRelationships.forEach(r => {
        foreigns[r.Foreign.Id] = [];
      });

      let promisesForeigns: Array<Promise<void>> = [];
      Object.getOwnPropertyNames(foreigns).forEach(foreign => {
        promisesForeigns.push(
          this.service.GetPaged(0, 1, '?CatalogDefinitionId=' + foreign).then(data => {
            foreigns[foreign] = data.Result;
          })
        );
      });

      Promise.all(promisesForeigns).then(done => {
        this.setState({ foreigns });
      });
    });
  }

  AFTER_CREATE = instance => {
    instance.CatalogDefinitionId = this.state.definition.Id;
    this.openDialog('catalog', instance);
  };

  ON_OPEN_ITEM = item => {
    this.openDialog('catalog', item);
  };

  render() {
    const { definition = {}, baseList, filterOptions, foreigns, isLoading } = this.state;
    return (
      <NoSsr>
        <Button variant='outlined' style={{ margin: 20, width: 220 }} onClick={() => this.navigateTo('/catalogs')}>
          <Icon>arrow_back</Icon>
          Catalogs
        </Button>
        <Container style={{ padding: 20 }} maxWidth='lg'>
          <Typography variant='h4' className='h4' gutterBottom>
            {definition.Name}
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
          <Paper style={{ overflowX: 'auto', width: '100%', overflowY: 'hidden' }}>
            <Table size='small'>
              <TableHead>
                <TableRow>
                  <TableCell style={{ width: 100 }} />
                  <TableCell>{definition.Name || 'Value'}</TableCell>
                  <TableCell>Hidden</TableCell>
                  {definition &&
                    definition.Fields &&
                    definition.Fields.map(field => <TableCell key={field.Id}>{field.FieldName}</TableCell>)}
                </TableRow>
              </TableHead>
              <TableBody>
                {!isLoading &&
                  baseList &&
                  baseList.map(item => (
                    <TableRow key={item.Id}>
                      <TableCell>
                        <Grid container direction='row' className='row' justify='center' alignItems='center' spacing={0}>
                          <Grid item xs>
                            <IconButton
                              color='default'
                              onClick={event => {
                                this.openItem(event, item);
                              }}
                              size='small'
                            >
                              <Icon>edit</Icon>
                            </IconButton>
                          </Grid>
                          <Grid item xs>
                            <IconButton
                              color='default'
                              onClick={event => {
                                this.removeItem(event, item);
                              }}
                              size='small'
                            >
                              <Icon>delete</Icon>
                            </IconButton>
                          </Grid>
                        </Grid>
                      </TableCell>
                      <TableCell>{item.Value}</TableCell>
                      <TableCell>{(item.Hidden || '').toString()}</TableCell>
                      {definition &&
                        definition.Fields &&
                        definition.Fields.map(field => {
                          item.Values = item.Values || [];
                          const fieldValue = item.Values.filter(v => v.FieldId == field.Id)
                            .map(v => v.Value)
                            .join(', ');

                          if (fieldValue) {
                            return <TableCell key={field.Id}>{fieldValue}</TableCell>;
                          } else {
                            return <TableCell key={field.Id} />;
                          }

                          //   if (field.Field.FieldType.startsWith('Relationship: Has One')) {
                          //     return <TableCell key={field.Id}>{item.ConvertedRelationships[field.FieldName] || ''}</TableCell>;
                          //   } else if (field.Field.startsWith('Relationship: Has Many')) {
                          //     return (
                          //       <TableCell key={field.FieldName}>{item.ConvertedRelationships['CSV' + field.FieldName] || ''}</TableCell>
                          //     );
                          //   } else
                          //     return (
                          //       <TableCell key={field.FieldName}>
                          //         {(item.ConvertedMeta[field.FieldName] && item.ConvertedMeta[field.FieldName].toString()) || ''}
                          //       </TableCell>
                          //     );
                          //
                        })}
                    </TableRow>
                  ))}
              </TableBody>
            </Table>
          </Paper>
        </Container>
        {/* <pre>{JSON.stringify(baseList, null, 3)}</pre> */}
        {/* <pre>{JSON.stringify(definition, null, 3)}</pre> */}
        <Dialog opener={this} id='catalog' okLabel='Save' dividersOff>
          {dialog => {
            return !isLoading && <Catalog dialog={dialog} data={this.state.catalog} definition={definition} foreigns={foreigns} />;
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

export default withRouter(CatalogsList);
