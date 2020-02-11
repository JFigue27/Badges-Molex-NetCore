import React from 'react';
import { withRouter } from 'next/router';
import { withSnackbar } from 'notistack';
import { NoSsr, Typography, Grid, ButtonBase } from '@material-ui/core';
import SearchBox from '../../widgets/Searchbox';
import Pagination from 'react-js-pagination';
import ListContainer, { ListProps } from '../../core/ListContainer';

import { Table } from '@material-ui/core';
import { TableHead } from '@material-ui/core';
import { TableBody } from '@material-ui/core';
import { TableRow } from '@material-ui/core';
import { TableCell } from '@material-ui/core';
import { Paper } from '@material-ui/core';
import { InputBase } from '@material-ui/core';
import { Button } from '@material-ui/core';
import { Icon } from '@material-ui/core';

import CommentService from './comment.service';
///start:slot:dependencies<<<
import RichTextEditor from '../../widgets/RichTextEditor';
import Dialog from '../../widgets/Dialog';
///end:slot:dependencies<<<

const service = new CommentService();
const defaultConfig = {
  service,
  ///start:slot:config<<<
  paginate: false,
  filterName: 'PRComments',
  sortName: 'PRComments'
  ///end:slot:config<<<
};

interface CommentsProps extends ListProps {}

class CommentsList extends ListContainer<CommentsProps> {
  constructor(props, config) {
    super(props, Object.assign(defaultConfig, config));
    ///start:slot:ctor<<<
    this.state.baseEntity = {};
    ///end:slot:ctor<<<
  }

  componentDidMount() {
    this.load(this.props.data);
    ///start:slot:load<<<///end:slot:load<<<
  }

  AFTER_LOAD = baseList => {
    ///start:slot:afterLoad<<<///end:slot:afterLoad<<<
  };

  AFTER_CREATE = instance => {
    ///start:slot:afterCreate<<<///end:slot:afterCreate<<<
  };

  AFTER_CREATE_AND_CHECKOUT = entity => {
    ///start:slot:afterCreateCheckout<<<///end:slot:afterCreateCheckout<<<
  };

  AFTER_REMOVE = entity => {
    ///start:slot:afterRemove<<<///end:slot:afterRemove<<<
    this.success('Comment Removed.');
    this.refresh();
  };

  ON_OPEN_ITEM = item => {
    ///start:slot:onOpenItem<<<///end:slot:onOpenItem<<<
  };

  ///start:slot:js<<<
  addComment = () => {
    this.openDialog('addComment', this.props.data).then(result => {
      let template = { ...this.props.data };
      template.CommentText = result.feedback;

      const baseList = [...this.state.baseList];
      baseList.push({ ...template, id: -1, CreatedBy: this.getCurrentUser().UserName, CreatedAt: new Date() }); //Quick local refresh
      this.setState({ baseList });
      this.saveItem(template).then(result => {
        this.setState({ baseEntity: {} }); //Clear dialog.
        this.refresh(); //Then refresh from server
      });
    });
  };
  ///end:slot:js<<<

  render() {
    const { isLoading, baseEntity, baseList, filterOptions, isDisabled } = this.state;

    ///start:slot:render<<<///end:slot:render<<<

    return (
      <NoSsr>
        <Typography variant='h5' className='h5' gutterBottom>
          Comments
        </Typography>
        <Paper style={{ width: '100%', overflowX: 'auto' }}>
          {baseList.length == 0 && (
            <Typography align='center' style={{ margin: 10 }}>
              (Empty)
            </Typography>
          )}
          <Table size='small'>
            <TableBody>
              {baseList &&
                baseList.map((item, index) => (
                  <TableRow key={item.Id}>
                    <TableCell style={{ width: 200 }}>
                      <Typography variant='caption' component='h6'>
                        {this.formatDate(item.CreatedAt)} {this.formatTime(item.CreatedAt)}
                      </Typography>
                      <Typography variant='caption' component='h6'>
                        {item.CreatedBy}
                      </Typography>
                      <Button
                        color='secondary'
                        size='small'
                        onClick={() => this.removeItem(null, item)}
                        style={{ padding: 0, fontSize: '.8em', minWidth: 0 }}
                      >
                        Remove
                      </Button>
                    </TableCell>
                    <TableCell className='RichTextInTable'>
                      <RichTextEditor value={item.CommentText} readOnly />
                    </TableCell>
                  </TableRow>
                ))}
            </TableBody>
          </Table>
        </Paper>
        <Grid container direction='row' className='row' justify='center' alignItems='flex-end' spacing={2} style={{ marginTop: 0 }}>
          <Grid item xs />
          <Grid item>
            <Button variant='contained' color='default' size='small' onClick={this.addComment}>
              Add Comment
            </Button>
          </Grid>
        </Grid>
        <Dialog opener={this} id='addComment' okLabel='Add' title='Add Comment'>
          {dialog => {
            dialog.onOk = () => {
              this.closeDialog('addComment', baseEntity.newComment);
            };
            return (
              <>
                <Grid container>
                  <Grid item xs>
                    <RichTextEditor
                      value={baseEntity.newComment || ''}
                      change={event => this.handleRichText(event, 'newComment')}
                      noButtons={['Image', 'SourceCode']}
                    />
                  </Grid>
                </Grid>
              </>
            );
          }}
        </Dialog>
      </NoSsr>
    );
  }
}

export default withSnackbar(withRouter(CommentsList) as any) as React.ComponentClass<CommentsProps>;
