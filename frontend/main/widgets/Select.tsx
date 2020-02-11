import React from 'react';
import clsx from 'clsx';
import Select from 'react-select';
import { emphasize, makeStyles, useTheme } from '@material-ui/core/styles';
import Typography from '@material-ui/core/Typography';
import NoSsr from '@material-ui/core/NoSsr';
import TextField from '@material-ui/core/TextField';
import Paper from '@material-ui/core/Paper';
import Chip from '@material-ui/core/Chip';
import ListItemText from '@material-ui/core/ListItemText';
import CancelIcon from '@material-ui/icons/Cancel';
// import Avatar from '@material-ui/core';
import PropTypes from 'prop-types';
import { List, ListItem } from '@material-ui/core';

const useStyles: any = makeStyles(theme => ({
  root: {
    flexGrow: 1,
    background: 'white',
    marginBottom: 0
  },
  input: {
    display: 'flex',
    padding: 0,
    // height: 'auto',
    height: 32
  },
  valueContainer: {
    display: 'flex',
    flexWrap: 'wrap',
    flex: 1,
    alignItems: 'center',
    overflow: 'hidden'
  },
  chip: {
    margin: theme.spacing(0.5, 0.25)
  },
  chipFocused: {
    backgroundColor: emphasize(theme.palette.type === 'light' ? theme.palette.grey[300] : theme.palette.grey[700], 0.08)
  },
  noOptionsMessage: {
    padding: theme.spacing(1, 2)
  },
  singleValue: {
    fontSize: 16
  },
  placeholder: {
    position: 'absolute',
    left: 2,
    bottom: 6,
    fontSize: '.8em'
  },
  paper: {
    position: 'absolute',
    zIndex: 5000,
    marginTop: theme.spacing(1),
    left: 0,
    right: 0
  },
  divider: {
    height: theme.spacing(2)
  }
}));

function NoOptionsMessage(props) {
  return (
    <Typography color='textSecondary' className={props.selectProps.classes.noOptionsMessage} {...props.innerProps}>
      {props.children}
    </Typography>
  );
}

NoOptionsMessage.propTypes = {
  children: PropTypes.node,
  innerProps: PropTypes.object,
  selectProps: PropTypes.object.isRequired
};

function inputComponent({ inputRef, ...props }) {
  return <div ref={inputRef} {...props} />;
}

inputComponent.propTypes = {
  inputRef: PropTypes.oneOfType([PropTypes.func, PropTypes.object])
};

function Control(props) {
  const {
    children,
    innerProps,
    innerRef,
    selectProps: { classes, TextFieldProps }
  } = props;

  return (
    <TextField
      fullWidth
      disabled={props.isDisabled}
      InputProps={{
        inputComponent,
        disableUnderline: props.disableUnderline,
        inputProps: {
          className: classes.input,
          ref: innerRef,
          children,
          ...innerProps
        }
      }}
      {...TextFieldProps}
    />
  );
}

Control.propTypes = {
  children: PropTypes.node,
  innerProps: PropTypes.object,
  innerRef: PropTypes.oneOfType([PropTypes.func, PropTypes.object]),
  selectProps: PropTypes.object.isRequired
};

function Option(props) {
  return (
    <ListItem
      dense
      ref={props.innerRef}
      selected={props.isFocused}
      style={{
        fontWeight: props.isSelected ? 500 : 400
      }}
      {...props.innerProps}
    >
      <ListItemText primary={props.children} secondary={props.data.secondaryText} />
    </ListItem>
  );
}

Option.propTypes = {
  children: PropTypes.node,
  innerProps: PropTypes.object,
  innerRef: PropTypes.oneOfType([PropTypes.func, PropTypes.object]),
  isFocused: PropTypes.bool,
  isSelected: PropTypes.bool
};

function Placeholder(props) {
  return (
    <Typography color='textSecondary' className={props.selectProps.classes.placeholder} {...props.innerProps}>
      {props.children}
    </Typography>
  );
}

Placeholder.propTypes = {
  children: PropTypes.node,
  innerProps: PropTypes.object,
  selectProps: PropTypes.object.isRequired
};

function SingleValue(props) {
  return (
    <Typography className={props.selectProps.classes.singleValue} {...props.innerProps}>
      {props.children}
    </Typography>
  );
}

SingleValue.propTypes = {
  children: PropTypes.node,
  innerProps: PropTypes.object,
  selectProps: PropTypes.object.isRequired
};

function ValueContainer(props) {
  return <div className={props.selectProps.classes.valueContainer}>{props.children}</div>;
}

ValueContainer.propTypes = {
  children: PropTypes.node,
  selectProps: PropTypes.object.isRequired
};

function MultiValue(props) {
  return (
    <Chip
      tabIndex={-1}
      label={props.children}
      className={clsx(props.selectProps.classes.chip, {
        [props.selectProps.classes.chipFocused]: props.isFocused
      })}
      onDelete={props.removeProps.onClick}
      deleteIcon={<CancelIcon {...props.removeProps} />}
      // avatar={<Avatar>{'AS'}</Avatar>}
    />
  );
}

MultiValue.propTypes = {
  children: PropTypes.node,
  isFocused: PropTypes.bool,
  removeProps: PropTypes.object.isRequired,
  selectProps: PropTypes.object.isRequired
};

function Menu(props) {
  return (
    <Paper square className={props.selectProps.classes.paper} {...props.innerProps}>
      <List>{props.children}</List>
    </Paper>
  );
}

Menu.propTypes = {
  children: PropTypes.node,
  innerProps: PropTypes.object,
  selectProps: PropTypes.object
};

const components = {
  Control,
  Menu,
  MultiValue,
  NoOptionsMessage,
  Option,
  Placeholder,
  SingleValue,
  ValueContainer
};

export default function IntegrationReactSelect(props) {
  const classes = useStyles();
  const theme = useTheme();
  // const [single, setSingle] = React.useState(null);
  // const [multi, setMulti] = React.useState(null);
  // const { options, keyProp, labelProp } = props;

  const selectStyles = {
    input: base => ({
      ...base,
      color: theme.palette.text.primary,
      '& input': {
        font: 'inherit'
      }
    })
  };

  const { keyProp = 'Id', labelProp = 'Value', emailProp = 'Email' } = props;

  const adaptOptions = opts => {
    let options = opts || [];
    if (options.length) {
      return options.map(item => {
        if (typeof item != 'object') {
          return {
            value: item,
            label: item,
            email: item
          };
        }
        return {
          ...item,
          value: item[keyProp],
          label: item[labelProp],
          email: item[emailProp]
        };
      });
    } else {
      return [];
    }
  };

  const adaptValue = () => {
    const item = props.value;
    if (props.flat) {
      return { ...item, label: props.value };
    } else {
      if (item)
        return {
          ...item,
          value: item[keyProp],
          label: item[labelProp],
          email: item[emailProp]
        };
      else return null;
    }
  };

  return (
    <NoSsr>
      <div className={classes.root} style={props.style}>
        <Select
          isClearable={props.clean}
          classes={classes}
          styles={selectStyles}
          inputId='react-select-single'
          TextFieldProps={{
            label: props.label,
            InputLabelProps: {
              htmlFor: 'react-select-single',
              shrink: true
            },
            placeholder: props.placeholder
          }}
          options={adaptOptions(props.options)}
          components={components}
          // value={props.flat ? { label: props.value } : props.value}
          value={adaptValue()}
          onChange={props.onChange}
          menuPlacement={props.placement}
          maxMenuHeight={props.height || 250}
          isDisabled={props.disabled}
        />
        {/* <Select
          classes={classes}
          styles={selectStyles}
          inputId="react-select-multiple"
          TextFieldProps={{
            // label: 'Countries',
            InputLabelProps: {
              htmlFor: 'react-select-multiple',
              shrink: true,
            },
            placeholder: 'Select multiple countries',
          }}
          components={components}
          value={multi}
          onChange={handleChangeMulti}
          isMulti
          options={(options || [])
            .map(opt => {
              return {
                value: opt[keyProp],
                label: opt[labelProp],
                avatar: (opt[labelProp] || '')
                  .split(' ')
                  .map(word => word.substring(0, 1).toUpperCase())
                  .join('')
              };
            })}
        /> */}
      </div>
    </NoSsr>
  );
}
