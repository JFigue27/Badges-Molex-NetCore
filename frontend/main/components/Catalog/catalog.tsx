import React from 'react';
import { withRouter } from 'next/router';
import { NoSsr, Typography, Grid, Divider } from '@material-ui/core';
import FormContainer, { FormProps } from '../../core/FormContainer';
import { Container } from '@material-ui/core';
import { TextField } from '@material-ui/core';

import CatalogService from './catalog.service';
import { FormControlLabel, Checkbox } from '@material-ui/core';
import Select from '../../widgets/Select';
import Chips from '../../widgets/Chips';
import { withSnackbar } from 'notistack';
import { EntryState } from '../../core/Contract';

const service = new CatalogService();
const defaultConfig = {
  service
};

interface CatalogProps extends FormProps {
  definition?: any;
  foreigns?: Array<any>;
}

class CatalogForm extends FormContainer<CatalogProps> {
  constructor(props, config) {
    super(props, Object.assign(defaultConfig, config));
  }

  componentDidMount() {
    this.load(this.props.data.Id ? this.props.data.Id : this.props.data);
  }

  render() {
    const { dialog } = this.props;
    if (dialog) dialog.onOk = this.onDialogOk;

    const { isLoading, isDisabled, baseEntity } = this.state;

    const { definition, foreigns = {} } = this.props;
    const { Fields } = definition || {};

    return (
      <NoSsr>
        <Container style={{ padding: 10 }}>
          <form noValidate onSubmit={this.onSubmit}>
            <button type='submit' style={{ display: 'none' }} />
            <Typography variant='h5' gutterBottom>
              {definition.Name}
            </Typography>
            <Grid container direction='row' spacing={3} alignItems='center'>
              <Grid item xs={12} sm>
                <TextField
                  type='text'
                  label={definition.Name}
                  value={baseEntity.Value || ''}
                  onChange={event => this.handleInputChange(event, 'Value')}
                  style={{ textAlign: 'left' }}
                  margin='normal'
                  fullWidth
                  variant='outlined'
                />
              </Grid>
              <Grid item>
                <FormControlLabel
                  style={{ marginTop: 20 }}
                  control={
                    <Checkbox
                      color='primary'
                      onChange={event => this.handleCheckBoxChange(event, 'Hidden')}
                      checked={baseEntity.Hidden == 1}
                      value={baseEntity.Hidden}
                    />
                  }
                  label='Hidden'
                  labelPlacement='end'
                />
              </Grid>

              {/* {Fields && Fields.length > 0 && (
            // <Divider style={{ margin: '20px 0' }} />
            // <Typography variant='h5' style={{ marginTop: 30 }}>
            //   Fields
            // </Typography>
          )} */}
              {Fields &&
                Fields.map(field => {
                  field.FieldType = field.FieldType || 'text';
                  baseEntity.Values = baseEntity.Values || [];
                  let fieldValue = baseEntity.Values.find(v => v.FieldId == field.Id);

                  if (!fieldValue) {
                    fieldValue = {
                      FieldId: field.Id
                    };

                    baseEntity.Values.push(fieldValue);
                  }

                  let ConvertedValue;

                  if (field.FieldType.startsWith('Relationship: Has One')) {
                    ConvertedValue = foreigns[field.ForeignId].find(e => fieldValue.IdsList && e.Id == fieldValue.IdsList[0]);
                    ConvertedValue = (ConvertedValue && ConvertedValue.Value) || '';
                    return (
                      <Grid item xs={12} key={field.Id}>
                        <Select
                          flat
                          label={field.FieldName}
                          value={ConvertedValue}
                          options={foreigns[field.ForeignId]}
                          onChange={event => {
                            fieldValue.IdsList = event ? [event.value] : [];
                            fieldValue.Value = null;
                            fieldValue.Entry_State = EntryState.Upserted;

                            this.setState({ baseEntity });
                          }}
                        />
                      </Grid>
                    );
                  }

                  if (field.FieldType.startsWith('Relationship: Has Many')) {
                    ConvertedValue = foreigns[field.ForeignId].filter(e => fieldValue.IdsList && fieldValue.IdsList.some(b => b == e.Id));

                    return (
                      <Grid item xs={12} key={field.Id}>
                        <Typography>{field.FieldName}</Typography>
                        <Chips
                          value={ConvertedValue}
                          options={foreigns[field.ForeignId]}
                          onChange={data => {
                            fieldValue.IdsList = data.map(e => e.Id);
                            fieldValue.Value = null;
                            fieldValue.Entry_State = EntryState.Upserted;

                            this.setState({ baseEntity });
                          }}
                        />
                      </Grid>
                    );
                  }

                  switch (field.FieldType.toLowerCase()) {
                    case 'checkbox':
                      ConvertedValue = fieldValue.Value == 'true' ? true : false;
                      return (
                        <Grid item xs={12} key={field.Id}>
                          <FormControlLabel
                            value={ConvertedValue || ''}
                            control={<Checkbox color='primary' checked={ConvertedValue || false} />}
                            label={field.FieldName}
                            labelPlacement='end'
                            onChange={(event: any) => {
                              ConvertedValue = (event.target as any).checked;
                              fieldValue.Value = String(ConvertedValue);
                              fieldValue.Entry_State = EntryState.Upserted;
                              this.setState({ baseEntity });
                            }}
                          />
                        </Grid>
                      );
                    default:
                      return (
                        <Grid item xs={12} key={field.Id}>
                          <TextField
                            type={field.FieldType}
                            label={field.FieldName}
                            value={fieldValue.Value || ''}
                            onChange={event => {
                              fieldValue.Value = event.target.value;
                              fieldValue.Entry_State = EntryState.Upserted;
                              fieldValue.IdsList = [];
                              this.setState({ baseEntity });
                            }}
                            InputLabelProps={{ shrink: true }}
                            margin='normal'
                            fullWidth
                          />
                        </Grid>
                      );
                  }
                })}
            </Grid>
            {/* <pre>{JSON.stringify(baseEntity, null, 3)}</pre> */}
            {/* <pre>{JSON.stringify(foreigns, null, 3)}</pre> */}
            <div style={{ height: 200 }} />
          </form>
        </Container>
      </NoSsr>
    );
  }
}

export default withSnackbar(withRouter(CatalogForm) as any) as React.ComponentClass<CatalogProps>;
