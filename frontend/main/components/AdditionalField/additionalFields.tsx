import React from 'react';
import { withRouter } from 'next/router';
import { NoSsr, Typography, Grid, FormControl, InputLabel, NativeSelect } from '@material-ui/core';
import SearchBox from '../../widgets/Searchbox';
import Pagination from 'react-js-pagination';
import ListContainer, { ListProps } from '../../core/ListContainer';
import { Table } from '@material-ui/core';
import { TableHead } from '@material-ui/core';
import { TableBody } from '@material-ui/core';
import { TableRow } from '@material-ui/core';
import { TableCell } from '@material-ui/core';
import { Button } from '@material-ui/core';
import { Icon } from '@material-ui/core';
import { InputBase } from '@material-ui/core';
import Select from '../../widgets/Select';
import { EntryState } from '../../core/Contract';

const service = {};
const defaultConfig = {
  service,
  autoAdd: true
};

interface AdditionalFieldsProps extends ListProps {
  foreigns: Array<any>;
  onChange(data: any): void;
}

class AdditionalFieldsList extends ListContainer<AdditionalFieldsProps> {
  constructor(props, config) {
    super(props, Object.assign(defaultConfig, config));
    this.tableRef = React.createRef();
    this.state.inputTypes = [
      '',
      'text',
      'number',
      'checkbox',
      'date',
      'datetime-local',
      'time',
      'color',
      'email',
      'image',
      'month',
      'password',
      'radio',
      'range',
      'tel',
      'url',
      'week',
      'Relationship: Has One',
      'Relationship: Has Many'
    ];
  }

  componentDidMount() {
    let baseList = (this.props as any).parent.Fields || [];
    baseList.push({});
    this.setState({
      baseList
    });

    this.AFTER_LOAD();
  }

  AFTER_LOAD = () => {
    this.enableCellNavigation(this.tableRef.current);
  };

  componentDidUpdate() {
    this.enableCellNavigation(this.tableRef.current);
  }

  ON_CHANGE = data => {
    const { foreigns } = this.props;

    data.forEach(field => {
      if (field.FieldType && field.FieldType.startsWith('Relationship')) {
        field.ForeignValue = '';
        if (field.ForeignId) {
          field.Foreign = foreigns.find(e => e.Id == field.ForeignId);
          if (field.Foreign) {
            field.ForeignValue = field.Foreign.Name;
          }
        }
      }
    });

    this.props.onChange(data);
  };

  render() {
    const { foreigns } = this.props;
    const { isDisabled } = this.state;

    return (
      <NoSsr>
        <Table size='small' ref={this.tableRef}>
          <TableHead>
            <TableRow>
              <TableCell>Field Name</TableCell>
              <TableCell style={{ width: 250 }}>Field Type</TableCell>
              <TableCell>Foreign</TableCell>
              <TableCell style={{ width: 100 }} />
            </TableRow>
          </TableHead>
          <TableBody>
            {this.state.baseList &&
              this.state.baseList.map((item, index) => (
                <TableRow key={index}>
                  <TableCell>
                    <InputBase
                      type='text'
                      className='filled'
                      autoComplete='off'
                      disabled={isDisabled}
                      readOnly={false}
                      onChange={event => this.handleInputChange(event, 'FieldName', index)}
                      value={item.FieldName || ''}
                      fullWidth
                    />
                  </TableCell>
                  <TableCell>
                    <FormControl fullWidth>
                      <NativeSelect value={item.FieldType} onChange={event => this.handleInputChange(event, 'FieldType', index)}>
                        {this.state.inputTypes.map(opt => (
                          <option key={opt} value={opt}>
                            {opt}
                          </option>
                        ))}
                      </NativeSelect>
                    </FormControl>
                  </TableCell>
                  <TableCell>
                    {item.FieldType && item.FieldType.startsWith('Relationship') && (
                      <FormControl fullWidth>
                        <NativeSelect value={item.ForeignId} onChange={event => this.handleInputChangeId(event, 'ForeignId', index)}>
                          <option value={undefined}></option>
                          {foreigns.map(opt => (
                            <option key={opt.Id} value={opt.Id}>
                              {opt.Name}
                            </option>
                          ))}
                        </NativeSelect>
                      </FormControl>
                    )}
                  </TableCell>
                  <TableCell>
                    <Grid container direction='row' justify='center' alignItems='center' spacing={0}>
                      <Grid item xs={12}>
                        {item.Entry_State != EntryState.Deleted && (
                          <Button
                            variant='contained'
                            color='default'
                            onClick={event => {
                              this.removeItemOnSave(event, index);
                            }}
                            size='small'
                          >
                            <Icon>delete</Icon>
                          </Button>
                        )}
                        {item.Entry_State == EntryState.Deleted && (
                          <Button
                            variant='text'
                            color='secondary'
                            onClick={event => {
                              this.cancelRemove(index);
                            }}
                            size='small'
                          >
                            Cancel Remove
                          </Button>
                        )}
                      </Grid>
                    </Grid>
                  </TableCell>
                </TableRow>
              ))}
          </TableBody>
        </Table>
      </NoSsr>
    );
  }
}

export default withRouter(AdditionalFieldsList);
