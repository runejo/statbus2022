import React from 'react'
import { Link } from 'react-router'
import { shape, number, string, func, array } from 'prop-types'
import { Table, Button } from 'semantic-ui-react'

import { formatDateTime } from 'helpers/dateHelper'

const LogItem = ({
  data: { id, unitName, unitType, issuedAt, resolvedAt, summaryMessages },
  localize,
}) => (
  <Table.Row>
    <Table.Cell className="wrap-content">{unitName}</Table.Cell>
    <Table.Cell className="wrap-content">{localize(unitType)}</Table.Cell>
    <Table.Cell className="wrap-content">{formatDateTime(issuedAt)}</Table.Cell>
    <Table.Cell className="wrap-content">
      {resolvedAt != null ? formatDateTime(resolvedAt) : '-'}
    </Table.Cell>
    <Table.Cell className="wrap-content">
      {summaryMessages.map(x => <p key={x}>{localize(x)}</p>)}
    </Table.Cell>
    <Table.Cell className="wrap-content">
      {resolvedAt == null && (
        <Button
          as={Link}
          to={`${window.location.pathname}/${id}`}
          content={localize('View')}
          icon="search"
          primary
        />
      )}
    </Table.Cell>
  </Table.Row>
)

LogItem.propTypes = {
  data: shape({
    id: number.isRequired,
    unitName: string.isRequired,
    unitType: string.isRequired,
    issuedAt: string.isRequired,
    resolvedAt: string,
    summaryMessages: array.isRequired,
  }).isRequired,
  localize: func.isRequired,
}

export default LogItem
