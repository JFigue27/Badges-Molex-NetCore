import { withStyles, createStyles } from '@material-ui/core/styles';
import { InputBase, NoSsr } from '@material-ui/core';
import { fade } from '@material-ui/core/styles/colorManipulator';

const styles = (theme: any) =>
  createStyles({
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
    inputRoot: {
      color: 'inherit',
      width: '100%'
    },
    inputInput: {
      paddingTop: theme.spacing(1),
      paddingRight: theme.spacing(1),
      paddingBottom: theme.spacing(1),
      paddingLeft: theme.spacing(1),
      // transition: theme.transitions.create('width'),
      width: '100%'
      // [theme.breakpoints.up('sm')]: {
      // width: 120,
      // '&:focus': {
      //   width: 200
      // }
      // }
    }
  });

const searchBox = props => {
  let { classes } = props;

  return (
    <NoSsr>
      <div className={classes.search}>
        <InputBase
          placeholder={props.placeholder}
          name={props.name}
          onChange={props.onChange}
          classes={{
            root: classes.inputRoot,
            input: classes.inputInput
          }}
          inputProps={{
            style: {
              width: props.width,
              textAlign: props.align
            }
          }}
        />
      </div>
    </NoSsr>
  );
};

export default withStyles(styles)(searchBox);
