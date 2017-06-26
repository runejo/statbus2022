import React from 'react'
import { Link } from 'react-router'
import { Button, Form, Loader, Message, Icon } from 'semantic-ui-react'
import R from 'ramda'

import DataAccess from 'components/DataAccess'
import RegionTree from 'components/RegionTree'
import { internalRequest } from 'helpers/request'
import { wrapper } from 'helpers/locale'
import styles from './styles'

const { func, shape } = React.PropTypes

class Edit extends React.Component {

  static propTypes = {
    fetchUser: func.isRequired,
    fetchRegionTree: func.isRequired,
    editForm: func.isRequired,
    submitUser: func.isRequired,
    localize: func.isRequired,
    regionTree: shape({}),
  }
  static defaultProps = {
    regionTree: undefined,
  }

  state = {
    rolesList: [],
    fetchingRoles: true,
    fetchingStandardDataAccess: true,
    rolesFailMessage: undefined,
  }

  componentDidMount() {
    this.props.fetchRegionTree()
    this.props.fetchUser(this.props.id)
    this.fetchRoles()
  }

  shouldComponentUpdate(nextProps, nextState) {
    return this.props.localize.lang !== nextProps.localize.lang
      || !R.equals(this.props, nextProps)
      || !R.equals(this.state, nextState)
  }

  fetchRoles = () => {
    internalRequest({
      url: '/api/roles',
      onSuccess: ({ result }) => {
        this.setState(({
          rolesList: result,
          fetchingRoles: false,
        }))
      },
      onFail: () => {
        this.setState(({
          rolesFailMessage: 'failed loading roles',
          fetchingRoles: false,
        }))
      },
    })
  }

  handleEdit = (e, { name, value }) => {
    this.props.editForm({ name, value })
  }

  handleSubmit = (e) => {
    e.preventDefault()
    this.props.submitUser(this.props.user)
  }

  handleCheck = value => this.props.editForm({ name: 'userRegions', value })

  renderForm() {
    const { user, localize, regionTree } = this.props
    return (
      <Form className={styles.form} onSubmit={this.handleSubmit}>
        <h2>{localize('EditUser')}</h2>
        <Form.Input
          value={user.name}
          onChange={this.handleEdit}
          name="name"
          label={localize('UserName')}
          placeholder={localize('RobertDiggs')}
          required
        />
        <Form.Input
          value={user.login}
          onChange={this.handleEdit}
          name="login"
          label={localize('UserLogin')}
          placeholder={localize('LoginPlaceholder')}
          required
        />
        <Form.Input
          value={user.newPassword || ''}
          onChange={this.handleEdit}
          name="newPassword"
          type="password"
          label={localize('UsersNewPassword')}
          placeholder={localize('TypeStrongPasswordHere')}
        />
        <Form.Input
          value={user.confirmPassword || ''}
          onChange={this.handleEdit}
          name="confirmPassword"
          type="password"
          label={localize('ConfirmPassword')}
          placeholder={localize('TypeNewPasswordAgain')}
          error={user.confirmPassword !== user.newPassword}
        />
        <Form.Input
          value={user.email}
          onChange={this.handleEdit}
          name="email"
          type="email"
          label={localize('UserEmail')}
          placeholder={localize('EmailPlaceholder')}
          required
        />
        <Form.Input
          value={user.phone}
          onChange={this.handleEdit}
          name="phone"
          type="tel"
          label={localize('UserPhone')}
          placeholder="555123456"
        />
        {this.state.fetchingRoles
          ? <Loader active />
          : <Form.Select
            value={user.assignedRoles}
            onChange={this.handleEdit}
            options={this.state.rolesList.map(r => ({ value: r.name, text: r.name }))}
            name="assignedRoles"
            label={localize('AssignedRoles')}
            placeholder={localize('SelectOrSearchRoles')}
            multiple
            search
          />}
        <DataAccess
          value={user.dataAccess}
          name="dataAccess"
          onChange={this.handleEdit}
          label={localize('DataAccess')}
        />
        {regionTree &&
        <RegionTree
          name="RegionTree"
          label="Regions"
          dataTree={regionTree}
          checked={user.userRegions}
          callBack={this.handleCheck}
        />}
        <Form.Input
          value={user.description}
          onChange={this.handleEdit}
          name="description"
          label={localize('Description')}
          placeholder={localize('NSO_Employee')}
        />
        <Button
          as={Link}
          to="/users"
          content={localize('Back')}
          icon={<Icon size="large" name="chevron left" />}
          floated="left"
          size="small"
          color="grey"
          type="button"
        />
        <Button
          content={localize('Submit')}
          floated="right"
          type="submit"
          primary
        />
        {this.state.rolesFailMessage
          && <div>
            <Message content={this.state.rolesFailMessage} negative />
            <Button onClick={() => { this.fetchRoles() }} type="button">
              {localize('TryReloadRoles')}
            </Button>
          </div>}
      </Form>
    )
  }

  render() {
    return (
      <div className={styles.userEdit}>
        {this.props.user !== undefined
          ? this.renderForm()
          : <Loader active />}
      </div>
    )
  }
}

export default wrapper(Edit)
