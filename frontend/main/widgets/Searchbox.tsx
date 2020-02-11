import { withStyles } from '@material-ui/core/styles';
import { InputBase, Icon, NoSsr, IconButton } from '@material-ui/core';
import SearchIcon from '@material-ui/icons/Search';
import { fade } from '@material-ui/core/styles/colorManipulator';

const styles: any = theme => ({
  search: {
    position: 'relative',
    borderRadius: theme.shape.borderRadius,
    backgroundColor: fade(theme.palette.common.white, 0.15),
    '&:hover': {
      backgroundColor: fade(theme.palette.common.white, 0.25)
    },
    marginLeft: 0,
    width: '100%',
    [theme.breakpoints.up('sm')]: {
      marginLeft: theme.spacing(1),
      width: 'auto'
    }
  },
  searchIcon: {
    width: theme.spacing(1) * 9,
    height: '100%',
    position: 'absolute',
    pointerEvents: 'none',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    left: 5
  },
  inputRoot: {
    color: 'inherit',
    width: '100%'
  },
  inputInput: {
    paddingTop: theme.spacing(1),
    paddingRight: theme.spacing(1),
    paddingBottom: theme.spacing(1),
    paddingLeft: theme.spacing(1) * 10,
    transition: theme.transitions.create('width'),
    width: '100%',
    [theme.breakpoints.up('sm')]: {
      width: 120,
      '&:focus': {
        width: 200
      }
    }
  }
});

const searchBox = props => {
  let { classes } = props;

  return (
    <NoSsr>
      <div className={classes.search}>
        <div className={classes.searchIcon}>
          <SearchIcon />
        </div>

        <InputBase
          placeholder='Search...'
          name='filterGeneral'
          onChange={event => {
            props.bindFilterInput(event);
          }}
          classes={{
            root: classes.inputRoot,
            input: classes.inputInput
          }}
          value={props.value || ''}
          startAdornment={
            props.value && (
              <IconButton color='inherit' size='small' onClick={props.clear} disableRipple disableFocusRipple>
                <Icon>clear</Icon>
              </IconButton>
            )
          }
        />
      </div>
    </NoSsr>
  );
};

export default withStyles(styles)(searchBox);
