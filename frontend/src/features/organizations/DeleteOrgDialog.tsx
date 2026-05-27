import { useMutation, useQueryClient } from '@tanstack/react-query'
import {
  AlertDialog, AlertDialogAction, AlertDialogCancel,
  AlertDialogContent, AlertDialogDescription,
  AlertDialogFooter, AlertDialogHeader, AlertDialogTitle,
} from '@/components/ui/alert-dialog'
import { orgApi } from './api'
import type { Organization } from './types'

interface Props {
  org: Organization | null
  onClose: () => void
}

export function DeleteOrgDialog({ org, onClose }: Props) {
  const qc = useQueryClient()
  const hasActiveChildren = (org?.children ?? []).some(c => c.isActive)

  const mutation = useMutation({
    mutationFn: () => orgApi.delete(org!.id),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['organizations'] }); onClose() },
  })

  return (
    <AlertDialog open={!!org} onOpenChange={v => !v && onClose()}>
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>Delete "{org?.name}"?</AlertDialogTitle>
          <AlertDialogDescription>
            {hasActiveChildren
              ? '⚠️ This organization has active child organizations. Remove or deactivate them first.'
              : 'This will soft-delete the organization (sets IsActive = false). All data is preserved.'}
          </AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
          <AlertDialogCancel onClick={onClose}>Cancel</AlertDialogCancel>
          {!hasActiveChildren && (
            <AlertDialogAction
              className="bg-laerdal-red hover:bg-laerdal-red-dark text-white"
              onClick={() => mutation.mutate()}
              disabled={mutation.isPending}
            >
              {mutation.isPending ? 'Deleting…' : 'Delete'}
            </AlertDialogAction>
          )}
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  )
}
