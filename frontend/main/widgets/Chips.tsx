import React from 'react';
import { Paper, Chip, NoSsr, Icon } from '@material-ui/core';
import Select from './Select';

interface ChipsProps {
  keyProp?: string;
  labelProp?: string;
  flat?: boolean;
  json?: boolean;
  options: Array<any>;
  value: string;
  onChange?(event: any): void;
  placeholder?: string;
  placement?: string;
  disabled?: boolean;
  readOnly?: boolean;
  noSelect?: boolean;
}

export default class Chips extends React.Component<ChipsProps> {
  state = {
    selected: [],
    selectValue: ''
  };

  onChange: any;

  adaptValues = value => {
    const { keyProp = 'Id', labelProp = 'Value', flat } = this.props;
    const { allOptions } = this.state as any;
    let items: Array<any> = [];

    items = typeof value == 'string' ? JSON.parse(value || '[]') : value || [];

    if (flat) {
      items = items.map(item => {
        let found = (allOptions || []).find(opt => opt[labelProp] == item);
        if (found) return found;
        return {
          [keyProp]: item,
          [labelProp]: item
        };
      });
    }

    if (items.length) {
      return items.map(item => {
        return {
          ...item,
          value: item[keyProp],
          label: item[labelProp]
        };
      });
    } else {
      return [];
    }
  };

  adaptOptions = opts => {
    const { keyProp = 'Id', labelProp = 'Value' } = this.props;
    let options = typeof opts == 'string' ? JSON.parse(opts || '[]') : opts || [];

    if (options.length) {
      return options.map(item => {
        if (typeof item != 'object') {
          return {
            [keyProp]: item,
            [labelProp]: item
          };
        }

        return {
          ...item,
          value: item[keyProp],
          label: item[labelProp]
        };
      });
    }

    return [];
  };

  adapterOut = selected => {
    const { keyProp = 'Id', labelProp = 'Value', json, flat } = this.props;

    let result = selected.map(item => {
      let adapted = { ...item };
      adapted[keyProp] = item.value;
      adapted[labelProp] = item.label;
      return adapted;
    });

    //Array of strings:
    if (flat) return result.map(r => r[labelProp]);

    //JSON as string:
    if (json) return JSON.stringify(result);

    //Array of objects:
    return result;
  };

  componentDidMount() {
    this.setState({
      allOptions: this.adaptOptions(this.props.options),
      selected: this.adaptValues(this.props.value)
    });
  }

  componentDidUpdate(prevProps) {
    const { options: prevOptions, value: prevValue } = prevProps;
    const { options, value } = this.props;

    if (!prevOptions && options) {
      this.setState({
        allOptions: this.adaptOptions(options)
      });
    }

    if ((!prevValue && value) || prevValue != value) {
      this.setState({
        selected: this.adaptValues(value)
      });
    }
  }

  handleSelectChange = item => {
    if (item == null) return;

    let { selected } = this.state as any;
    selected.push(item);
    this.setState({ selected, selectValue: '' });

    this.onChange(this.adapterOut(selected));
  };

  onRemove = index => {
    let { selected } = this.state;
    selected.splice(index, 1);
    this.setState({ selected });
    this.onChange(this.adapterOut(selected));
  };

  render() {
    const { onChange, placeholder, placement, disabled, readOnly, noSelect } = this.props;
    const { selected, allOptions } = this.state as any;
    const { keyProp = 'Id', labelProp = 'Value' } = this.props;
    this.onChange = onChange;

    return (
      <>
        <NoSsr>
          {/* <pre>{JSON.stringify(allOptions, null, 3)}</pre>
          <pre>{JSON.stringify(selected, null, 3)}</pre> */}
          <Paper style={{ minHeight: 32, marginTop: 0 }} className='Chips' elevation={0}>
            {(!selected || selected.length == 0) && (disabled || readOnly || noSelect) && <span style={{ fontSize: '.7em' }}>(Empty)</span>}
            {selected.map((item, index: number) => {
              return (
                <Chip
                  size='small'
                  key={index}
                  color='default'
                  variant='default'
                  label={item.label}
                  className={`Person-Chip ${item.classes || ''}`}
                  onDelete={disabled || readOnly || item.fixedChip ? undefined : () => this.onRemove(index)}
                  icon={item.icon && <Icon style={{ color: 'inherit', fontSize: '1.2em' }}>{item.icon}</Icon>}
                  disabled={disabled}
                />
              );
            })}
            {!disabled && !readOnly && !noSelect && (
              <Select
                options={(allOptions || []).filter(opt => !selected.some(s => s[labelProp] == opt[labelProp]))}
                onChange={this.handleSelectChange}
                placement={placement || 'top'}
                label={''}
                value={this.state.selectValue || ''}
                style={{ width: 200, maxWidth: 200, display: 'inline-block' }}
                keyProp={keyProp}
                labelProp={labelProp}
              />
            )}
          </Paper>
        </NoSsr>
      </>
    );
  }
}
