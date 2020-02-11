import React from 'react';
import { makeStyles } from '@material-ui/core/styles';
import ExpansionPanel from '@material-ui/core/ExpansionPanel';
import ExpansionPanelDetails from '@material-ui/core/ExpansionPanelDetails';
import ExpansionPanelSummary from '@material-ui/core/ExpansionPanelSummary';
// import ExpandMoreIcon from '@material-ui/icons/ExpandMore';
import ExpandMoreIcon from '@material-ui/icons/Add';
import ExpandLessIcon from '@material-ui/icons/Remove';

const useStyles = makeStyles(theme => ({
  root: {
    width: '100%'
  },
  heading: {
    fontSize: theme.typography.pxToRem(15),
    flexBasis: '33.33%',
    flexShrink: 0
  },
  secondaryHeading: {
    fontSize: theme.typography.pxToRem(15),
    color: theme.palette.text.secondary
  }
}));

interface ExpansionProps {
  panel?: any;
  children?: any;
}

const ControlledExpansionPanels = (props: ExpansionProps) => {
  const classes = useStyles();
  const [expanded, setExpanded] = React.useState<any>(false);

  const handleChange = panel => (event, isExpanded) => {
    setExpanded(isExpanded ? panel : false);
  };

  return (
    <div className={classes.root}>
      {/* <pre>{JSON.stringify(props, null, 3)}</pre> */}
      <ExpansionPanel expanded={expanded === 'panel1'} onChange={handleChange('panel1')}>
        <ExpansionPanelSummary
          expandIcon={
            expanded ? (
              <ExpandLessIcon style={{ background: '#080808', borderRadius: 50, color: '#f2f2f2' }} />
            ) : (
              <ExpandMoreIcon style={{ background: '#d9d9d9', borderRadius: 50, color: '#fff' }} />
            )
          }
          aria-controls='panel1bh-content'
          id='panel1bh-header'
        >
          {props.panel}
        </ExpansionPanelSummary>
        <ExpansionPanelDetails style={{ background: '#f7f5f6' }}>{expanded && props.children}</ExpansionPanelDetails>
      </ExpansionPanel>
    </div>
  );
};
export default ControlledExpansionPanels;
