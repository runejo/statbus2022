import React from 'react'
import { Link } from 'react-router'
import { Button, Loader, Message, List } from 'semantic-ui-react'

import { systemFunction as sF } from '../../../helpers/checkPermissions'
import styles from './styles'

const Item = ({ id, deleteStatUnit, ...statUnit }) => {
  const handleDelete = () => {
    if (confirm(`Delete StatUnit '${statUnit.ShortName}'. Are you sure?`)) deleteStatUnit(id)
  }
  return (
    <List.Item>
      <List.Icon name="statUnit" size="large" verticalAlign="middle" />
      <List.Content>
        <List.Header
          content={sF('StatUnitDelete')
            ? <Link to={`/StatUnits/edit/${id}`}>{statUnit.shortName}</Link>
            : <span>{statUnit.shortName}</span>}
        />
        <List.Description>
          <span>{statUnit.name}</span>
          {sF('StatUnitDelete') && <Button onClick={handleDelete} negative>delete</Button>}
        </List.Description>
      </List.Content>
    </List.Item>
  )
}

export default class StatUnitsList extends React.Component {
  componentDidMount() {
    this.props.fetchStatUnits()
  }
  render() {
    const { statUnits, totalCount, totalPages, deleteStatUnit, status } = this.props
    return (
      <div>
        <h2>StatUnits list</h2>
        <div className={styles['list-root']}>
          {sF('StatUnitCreate') && <Link to="/statunits/create">Create</Link>}
          <Loader active={status === 1} />
          <List>
            {statUnits && statUnits.map(u =>
              <Item key={u.id} {...u} deleteStatUnit={deleteStatUnit} />)}
          </List>
          <span>total: {totalCount}</span>
          <span>total pages: {totalPages}</span>
        </div>
      </div>
    )
  }
}
