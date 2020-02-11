import React from 'react';
import { withRouter } from 'next/router';
import { NoSsr, Typography, Grid } from '@material-ui/core';
import FormContainer, { FormProps } from '../../core/FormContainer';
import { Container } from '@material-ui/core';
import { TextField } from '@material-ui/core';

import CatalogDefinitionService from './catalogdefinition.service';
import AdditionalFields from '../AdditionalField/additionalFields';
import { withSnackbar } from 'notistack';

const service = new CatalogDefinitionService();
const defaultConfig = {
  service
};

interface CatalogDefinitionProps extends FormProps {
  // dialog?: any;
  // data?: any;
  // onChange?(data: any, field: string): void;
}

class CatalogDefinitionForm extends FormContainer<CatalogDefinitionProps> {
  constructor(props, config) {
    super(props, Object.assign(defaultConfig, config));
  }

  componentDidMount() {
    this.load(this.props.data.Id > 0 ? this.props.data.Id : this.props.data);
    this.service.LoadEntities().then(foreigns => {
      this.setState({ foreigns });
    });
  }

  onFieldsChange = data => {
    let baseEntity = this.state.baseEntity;
    baseEntity.Fields = data;
    this.setState({ baseEntity });
  };

  // onRelationshipsChange = data => {
  //   let baseEntity = this.state.baseEntity;
  //   baseEntity.CatalogDefinitionRelationships = data;
  //   this.setState({ baseEntity });
  // };

  render() {
    const { dialog } = this.props as any;
    if (dialog) dialog.onOk = this.onDialogOk;

    const { baseEntity, foreigns = [], isDisabled } = this.state as any;

    return (
      <NoSsr>
        <Container style={{ padding: 20 }}>
          <TextField
            type='text'
            label='Name'
            value={baseEntity.Name || ''}
            onChange={event => this.handleInputChange(event, 'Name')}
            style={{ textAlign: 'left' }}
            margin='normal'
            fullWidth
          />
          <Typography variant='h5' style={{ marginTop: 30 }}>
            Fields
          </Typography>
          {baseEntity.Name && <AdditionalFields parent={baseEntity} onChange={this.onFieldsChange} foreigns={foreigns} />}
          {/* <pre>{this.stringify(baseEntity)}</pre> */}
          <div style={{ height: 250 }} />
        </Container>
      </NoSsr>
    );
  }
}

export default withSnackbar(withRouter(CatalogDefinitionForm) as any) as React.ComponentClass<CatalogDefinitionProps>;
