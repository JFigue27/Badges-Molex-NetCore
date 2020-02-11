import React, { useState } from 'react';
import Card from '@material-ui/core/Card';
import TextField from '@material-ui/core/TextField';
import Fab from '@material-ui/core/Fab';
import Lock from '@material-ui/icons/Lock';
import Fingerprint from '@material-ui/icons/Fingerprint';
import Icon from '@material-ui/core/Icon';
import CardActions from '@material-ui/core/CardActions';
import CardContent from '@material-ui/core/CardContent';
import Link from 'next/link';
import Button from '@material-ui/core/Button';
import Typography from '@material-ui/core/Typography';
import { checkPropTypes } from 'prop-types';
import AuthService from '../core/AuthService';

const Login = props => {
  const [username, setUsername] = useState();
  const [password, setPassword] = useState();
  const [error, setError] = useState();

  const login = async event => {
    event.preventDefault();
    try {
      await AuthService.login(username, password);
      props.onCloseLogin();
    } catch (e) {
      setError(e);
    }
  };
  return (
    <div className='container-fluid'>
      <div className='login'>
        <Card className='card'>
          <form onSubmit={login}>
            <CardContent>
              <Typography style={{ margin: '40px 5px' }} variant='h4' align='center' color='inherit'>
                Main
              </Typography>
              {/* <div className="finger">
                <Fingerprint style={{ fontSize: 80 }} />
              </div> */}
              <div className='text-field'>
                <TextField required fullWidth label='User' margin='normal' onChange={event => setUsername(event.target.value)} />
              </div>
              <div className='text-field'>
                <TextField
                  required
                  fullWidth
                  label='Password'
                  type='password'
                  autoComplete='current-password'
                  margin='normal'
                  onChange={event => setPassword(event.target.value)}
                />
              </div>
              {error && <p>{error}</p>}
              <div className='botton'>
                <Fab variant='extended' aria-label='Delete' type='submit'>
                  <Lock />
                  Login
                </Fab>
              </div>
            </CardContent>
          </form>
        </Card>
      </div>
      <style jsx>{`
        .container-fluid {
          display: grid;
          justify-content: center;
        }
        .login {
          padding-top: 30%;
          width: 350px;
        }
        .botton {
          padding: 50px 0 50px;
          text-align: center;
        }
        .finger {
          padding-top: 15px;
          text-align: center;
        }
        .image {
          display: inline-grid;
        }
        img {
          width: 100%;
          min-width: 180px;
          height: auto;
          display: inline-block;
          margin: 0 auto;
          padding-top: 4px;
        }
        .text-field {
          text-align: center;
        }
        .card {
          min-width: 275;
          background: white;
        }
        h1 {
          text-align: center;
        }
      `}</style>
      <style jsx global>{`
        body {
          margin: 0;
          font-family: system-ui;
          background: #0000000a;
        }
      `}</style>
    </div>
  );
};

export default Login;
